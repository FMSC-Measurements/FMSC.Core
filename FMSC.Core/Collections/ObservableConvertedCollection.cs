using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;

namespace FMSC.Core.Collections
{
    public class ObservableConvertedCollection<TIn, TOut> : IReadOnlyList<TOut>, IReadOnlyCollection<TOut>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event NotifyCollectionChangedEventHandler PreviewCollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ObservableCollection<TIn> _Source;
        private readonly ObservableCollection<TOut> _EditableCollection;
        private Dictionary<TIn, TOut> _ConvertedLookup;
        private Func<TIn, TOut> _Converter;

        private bool _disposed;
        public bool Disposed { get { lock (this) return _disposed; } }
        

        public int Count { get { return _EditableCollection.Count; } }
        
        public TOut this[int index]
        {
            get { return _EditableCollection[index]; }
        }
        

        public ObservableConvertedCollection(ObservableCollection<TIn> source, Func<TIn, TOut> converter) : base()
        {
            _Source = source;
            _Converter = converter;

            List<TOut> osource = new List<TOut>();
            _ConvertedLookup = new Dictionary<TIn, TOut>();

            foreach (TIn i in _Source)
            {
                TOut o = _Converter(i);
                _ConvertedLookup.Add(i, o);
                osource.Add(o);
            }

            _EditableCollection = new ObservableCollection<TOut>(osource);

            ((INotifyCollectionChanged)_Source).CollectionChanged += Source_CollectionChanged;
            ((INotifyPropertyChanged)_Source).PropertyChanged += OnPropertyChanged;
        }

        ~ObservableConvertedCollection()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ((INotifyCollectionChanged)_Source).CollectionChanged -= Source_CollectionChanged;
                ((INotifyPropertyChanged)_Source).PropertyChanged -= OnPropertyChanged;

                _EditableCollection.Clear();
            }

            _disposed = true;
        }


        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (this)
            {
                NotifyCollectionChangedEventArgs cvte = null;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            cvte = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                e.NewItems.Cast<TIn>().Select(i => _ConvertedLookup[i]).First(),
                                e.NewStartingIndex
                            );
                            break;
                        }
                    case NotifyCollectionChangedAction.Move:
                        {
                            cvte = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Move,
                                e.OldItems.Cast<TIn>().Select(i => _ConvertedLookup[i]).First(),
                                e.NewStartingIndex,
                                e.OldStartingIndex
                            );
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            cvte = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Remove,
                                e.OldItems.Cast<TIn>().Select(i => _ConvertedLookup[i]).First(),
                                e.OldStartingIndex
                            );
                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            cvte = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                e.OldItems.Cast<TIn>().Select(i => _ConvertedLookup[i]).First(),
                                e.NewItems.Cast<TIn>().Select(i => _ConvertedLookup[i]).First(),
                                e.NewStartingIndex
                            );
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            cvte = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                            break;
                        }
                }

                OnPreviewCollectionChanged(cvte);

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            List<TOut> newItems = new List<TOut>();

                            if (e.NewItems.Count > 1)
                            {
                                int index = e.NewStartingIndex;

                                foreach (TIn i in e.NewItems)
                                {
                                    TOut o = _Converter(i);
                                    _ConvertedLookup.Add(i, o);
                                    _EditableCollection.Insert(index, o);
                                    newItems.Add(o);
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
                                    newItems.Add(o);
                                }
                                else
                                {
                                    newItems.Add(_ConvertedLookup[i]);
                                }
                            }

                            cvte = new NotifyCollectionChangedEventArgs(e.Action, newItems, e.NewStartingIndex);
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            List<TOut> oldItems = new List<TOut>();

                            foreach (TIn ti in e.OldItems)
                            {
                                if (_ConvertedLookup.ContainsKey(ti))
                                {
                                    TOut o = _ConvertedLookup[ti];
                                    _EditableCollection.Remove(o);
                                    _ConvertedLookup.Remove(ti);
                                    oldItems.Add(o);
                                }
                            }

                            cvte = new NotifyCollectionChangedEventArgs(e.Action, oldItems, e.OldStartingIndex);
                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            List<TOut> newItems = new List<TOut>();
                            List<TOut> oldItems = new List<TOut>();

                            TIn ni = (TIn)e.NewItems[0];
                            TIn oi = (TIn)e.OldItems[0];

                            if (_ConvertedLookup.ContainsKey(oi))
                            {
                                TOut o = _ConvertedLookup[oi];
                                _ConvertedLookup.Remove(oi);
                                oldItems.Add(o);
                            }

                            TOut nci = _Converter(ni);
                            _ConvertedLookup.Add(ni, nci);
                            newItems.Add(nci);

                            _EditableCollection[e.NewStartingIndex] = nci;

                            cvte = new NotifyCollectionChangedEventArgs(e.Action, newItems, oldItems, e.NewStartingIndex);
                            break;
                        }
                    case NotifyCollectionChangedAction.Move:
                        {
                            TOut o = _EditableCollection[e.OldStartingIndex];
                            _EditableCollection.Move(e.OldStartingIndex, e.NewStartingIndex);

                            cvte = new NotifyCollectionChangedEventArgs(e.Action, o, e.NewStartingIndex, e.OldStartingIndex);
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            _EditableCollection.Clear();
                            cvte = new NotifyCollectionChangedEventArgs(e.Action);
                            break;
                        }
                    default:
                        break;
                }

                OnCollectionChanged(cvte);
            }
        }


        protected virtual void OnPreviewCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            PreviewCollectionChanged?.Invoke(this, args);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
        
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
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
