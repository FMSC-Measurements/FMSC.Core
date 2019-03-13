using CSUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FMSC.Core.Collections
{
    public class ObservableFilteredCollection<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        public readonly bool ItemCanChange;

        Func<T, bool> _Filter;

        private ReadOnlyObservableCollection<T> _Source;
        private ObservableCollection<T> _Collection;

        private object locker = new object();

        public int Count { get { return _Collection.Count; } }


        public ObservableFilteredCollection(ReadOnlyObservableCollection<T> source, Func<T, bool> filter)
        {
            ItemCanChange = typeof(T).ImplementsInterface<INotifyPropertyChanged>();

            _Source = source;
            _Filter = filter;

            lock (locker)
            {
                _Collection = new ObservableCollection<T>(_Source.Where(item => _Filter(item)));

                foreach (T item in _Source)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += Item_PropertyChanged;
                }
            }

            ((INotifyCollectionChanged)source).CollectionChanged += Source_CollectionChanged;
            ((INotifyCollectionChanged)_Collection).CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCollectionChanged);
            ((INotifyPropertyChanged)_Collection).PropertyChanged += new PropertyChangedEventHandler(HandlePropertyChanged);
        }
        


        public T this[int index]
        {
            get { return _Collection[index]; }
        }
        

        private int GetInsertIndex(T item)
        {
            return GetInsertIndex(_Source.IndexOf(item));
        }

        private int GetInsertIndex(int newStartingIndex)
        {
            int index = 0;
            if (_Source.Count != 0 && newStartingIndex != 0)
            {
                index = (newStartingIndex >= _Collection.Count) ? _Collection.Count - 1 : newStartingIndex;

                T compItem;

                for (int i = newStartingIndex - 1; i > 0; i--)
                {
                    compItem = _Source[i];
                    if (_Filter(compItem))
                    {
                        for (; index > -1; index--)
                        {
                            if (_Collection[index].Equals(compItem))
                            {
                                index++;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            if (index < 0)
                index = _Collection.Count;

            return index;
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (locker)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            int index = GetInsertIndex(e.NewStartingIndex);

                            foreach (T item in e.NewItems)
                            {
                                if (ItemCanChange)
                                    ((INotifyPropertyChanged)item).PropertyChanged += Item_PropertyChanged;

                                if (_Filter(item))
                                {
                                    _Collection.Insert(index, item);
                                    index++;
                                }
                            }
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            foreach (T item in e.OldItems)
                            {
                                ((INotifyPropertyChanged)item).PropertyChanged -= Item_PropertyChanged;
                                _Collection.Remove(item);
                            }
                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            T oi = (T)e.OldItems[0];
                            if (_Filter(oi))
                            {
                                _Collection[_Collection.IndexOf(oi)] = (T)e.NewItems[0];
                            }
                            break;
                        }
                    case NotifyCollectionChangedAction.Move:
                        {
                            _Collection.Move(e.OldStartingIndex, e.NewStartingIndex);
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            if (ItemCanChange)
                            {
                                foreach (T item in _Collection)
                                {
                                    ((INotifyPropertyChanged)item).PropertyChanged -= Item_PropertyChanged;
                                }
                            }
                            _Collection.Clear();
                            break;
                        }
                    default:
                        break;
                }
            }
        }


        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            T item = (T)sender;

            if (_Filter(item))
            {
                if (!_Collection.Contains(item))
                    _Collection.Insert(GetInsertIndex(item), item);
            }
            else
            {
                _Collection.Remove(item);
            }
        }


        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
        
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }


        public int IndexOf(T item)
        {
            return _Collection.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return _Collection.Contains(item);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }
    }
}
