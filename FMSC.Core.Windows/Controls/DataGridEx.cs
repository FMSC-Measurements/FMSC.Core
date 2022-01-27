using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FMSC.Core.Windows.Controls
{
    public delegate void ListChangedEvent(IList items);

    public class DataGridEx : DataGrid
    {
        public event ListChangedEvent SelectedItemListChanged;
        public event ListChangedEvent VisibleItemListChanged;

        public event EventHandler Sorted;

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(DataGridEx), new PropertyMetadata(null));

        public IList VisibleItemsList
        {
            get { return (IList)GetValue(VisibleItemsListProperty); }
            set { SetValue(VisibleItemsListProperty, value); }
        }

        public static readonly DependencyProperty VisibleItemsListProperty =
                DependencyProperty.Register("VisibleItemsList", typeof(IList), typeof(DataGridEx), new PropertyMetadata(null));

        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionUpdated;


        public ObservableCollection<DataGridColumn> BindableColumns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(BindableColumnsProperty); }
            set { SetValue(BindableColumnsProperty, value); }
        }
        
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.Register("BindableColumns",
                typeof(ObservableCollection<DataGridColumn>),
                typeof(DataGridEx),
                new UIPropertyMetadata(null, BindableColumnsPropertyChanged)
            );

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = source as DataGrid;
            ObservableCollection<DataGridColumn> newColumns = e.NewValue as ObservableCollection<DataGridColumn>;

            NotifyCollectionChangedEventHandler ncceh;
            Action<object, NotifyCollectionChangedEventArgs> onCollectionChanged = (sender, ne) =>
            {
                if (ne.Action == NotifyCollectionChangedAction.Reset)
                {
                    dataGrid.Columns.Clear();
                    if (ne.NewItems != null)
                    {
                        foreach (DataGridColumn column in ne.NewItems)
                        {
                            dataGrid.Columns.Add(column);
                        }
                    }
                }
                else if (ne.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (DataGridColumn column in ne.NewItems)
                    {
                        dataGrid.Columns.Add(column);
                    }
                }
                else if (ne.Action == NotifyCollectionChangedAction.Move)
                {
                    dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                }
                else if (ne.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (DataGridColumn column in ne.OldItems)
                    {
                        dataGrid.Columns.Remove(column);
                    }
                }
                else if (ne.Action == NotifyCollectionChangedAction.Replace)
                {
                    dataGrid.Columns[ne.NewStartingIndex] = ne.NewItems[0] as DataGridColumn;
                }
            };

            ncceh = new NotifyCollectionChangedEventHandler(onCollectionChanged);


            if (e.OldValue != null)
            {
                ObservableCollection<DataGridColumn> oldColumns = e.OldValue as ObservableCollection<DataGridColumn>;
                oldColumns.CollectionChanged -= ncceh;
            }

            dataGrid.Columns.Clear();

            if (newColumns == null)
                return;

            foreach (DataGridColumn column in newColumns)
                dataGrid.Columns.Add(column);

            newColumns.CollectionChanged += ncceh;

            RoutedEventHandler reh = null;

            Action<object, RoutedEventArgs> dgUnload = (s, re) =>
            {
                newColumns.CollectionChanged -= ncceh;
                dataGrid.Unloaded -= reh;
            };

            reh = new RoutedEventHandler(dgUnload);

            dataGrid.Unloaded += reh;
        }

        private CollectionView _CurrentSource;

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (_CurrentSource != null)
            {
                ((INotifyCollectionChanged)_CurrentSource).CollectionChanged -= DataGridEx_CollectionChanged;
                _CurrentSource = null;
            }

            if (newValue is CollectionView cv && cv != null)
            {
                _CurrentSource = cv;

                ((INotifyCollectionChanged)_CurrentSource).CollectionChanged += DataGridEx_CollectionChanged;

                this.VisibleItemsList = this.ItemContainerGenerator.Items;
                VisibleItemListChanged?.Invoke(VisibleItemsList);
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        private void DataGridEx_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionUpdated?.Invoke(sender, e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.SelectedItemsList = this.SelectedItems;
            SelectedItemListChanged?.Invoke(SelectedItemsList);
        }

        protected override void OnSorting(DataGridSortingEventArgs e)
        {
            base.OnSorting(e);

            OnSorted();
        }

        protected void OnSorted()
        {
            Sorted?.Invoke(this, new EventArgs());
        }



    }
}
