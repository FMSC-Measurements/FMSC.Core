using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FMSC.Core.Collections
{
    public class ObservableConvertedCollection<TIn, TOut> : IReadOnlyList<TOut>, IReadOnlyCollection<TOut>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { CollectionChanged += value; }
            remove { CollectionChanged -= value; }
        }

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        
        private ObservableCollection<TOut> _EditableCollection;
        private Dictionary<TIn, TOut> _ConvertedLookup;
        private Func<TIn, TOut> _Converter;

        private object locker = new object();
        

        public int Count { get { return _EditableCollection.Count; } }
        
        public TOut this[int index]
        {
            get { return _EditableCollection[index]; }
        }
        

        public ObservableConvertedCollection(ObservableCollection<TIn> source, Func<TIn, TOut> converter) : base()
        {
            _Converter = converter;

            List<TOut> osource = new List<TOut>();
            _ConvertedLookup = new Dictionary<TIn, TOut>();

            foreach (TIn i in source)
            {
                TOut o = _Converter(i);
                _ConvertedLookup.Add(i, o);
                osource.Add(o);
            }

            _EditableCollection = new ObservableCollection<TOut>(osource);

            ((INotifyCollectionChanged)source).CollectionChanged += Source_CollectionChanged;

            ((INotifyCollectionChanged)_EditableCollection).CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCollectionChanged);
            ((INotifyPropertyChanged)_EditableCollection).PropertyChanged += new PropertyChangedEventHandler(HandlePropertyChanged);
        }


        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (locker)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems.Count > 1)
                        {
                            int index = e.NewStartingIndex;

                            foreach (TIn i in e.NewItems)
                            {
                                TOut o = _Converter(i);
                                _ConvertedLookup.Add(i, o);
                                _EditableCollection.Insert(index, o);
                                index++;
                            }
                        }
                        else
                        {
                            TIn i = (TIn)e.NewItems[0];
                            TOut o = _Converter(i);
                            if (!_ConvertedLookup.ContainsKey(i))
                            {
                                _ConvertedLookup.Add(i, o);
                                _EditableCollection.Insert(e.NewStartingIndex, o);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (TIn ti in e.OldItems)
                        {
                            if (_ConvertedLookup.ContainsKey(ti))
                            {
                                _EditableCollection.Remove(_ConvertedLookup[ti]);
                                _ConvertedLookup.Remove(ti);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        TIn ni = (TIn)e.NewItems[0];
                        TIn oi = (TIn)e.OldItems[0];

                        _ConvertedLookup.Remove(oi);

                        TOut nci = _Converter(ni);
                        _ConvertedLookup.Add(ni, nci);

                        _EditableCollection[e.NewStartingIndex] = nci;
                        break;
                    case NotifyCollectionChangedAction.Move:
                        _EditableCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _EditableCollection.Clear();
                        break;
                    default:
                        break;
                } 
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
        

        public int IndexOf(TOut item)
        {
            return _EditableCollection.IndexOf(item);
        }
        
        public bool Contains(TOut item)
        {
            return _EditableCollection.Contains(item);
        }
        

        public IEnumerator<TOut> GetEnumerator()
        {
            return _EditableCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _EditableCollection.GetEnumerator();
        }
    }
}
