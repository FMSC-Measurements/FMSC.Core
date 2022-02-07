using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace FMSC.Core.ComponentModel
{
    /// <summary>
    /// Notifies clients that a property value has changed using set and setfield functions
    /// </summary>
#if DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public abstract class BaseModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Event for when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<String, Object> _PropertyValues;
        internal Dictionary<String, Object> PropertyValues => (_PropertyValues ?? (_PropertyValues = new Dictionary<string, object>()));

        public bool Disposed { get; private set; }

        ~BaseModel()
        {
            Dispose(false);
        }


        /// <summary>
        /// Triggers Property Changed Event
        /// </summary>
        /// <param name="propertyName">Name of Property</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Triggers Property Changed Event for each property name
        /// </summary>
        /// <param name="propertyNames">Names of Properties</param>
        protected virtual void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (String propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        /// <summary>
        /// Triggers Property Changed Event for each property name
        /// </summary>
        /// <param name="propertyNames">Names of Properties</param>
        protected virtual void OnPropertyChanged(IEnumerable<string> propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (String propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="field">Reference Field</param>
        /// <param name="value">Value of Property</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Whether the property's value has changed</returns>
        protected virtual bool SetField<T>(ref T field, T value, string propertyName)
        {
            return SetField(ref field, value, null, propertyName);
        }

        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="field">Reference Field</param>
        /// <param name="value">Value of Property</param>
        /// <param name="action">Action to be done if the property has changed</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Whether the property's value has changed</returns>
        protected virtual bool SetField<T>(ref T field, T value, Action action = null, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;

            OnPropertyChanged(propertyName);

            action?.Invoke();

            return true;
        }
        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="value">Value of Property</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Whether the property's value has changed</returns>
        protected virtual bool Set<T>(T value, string propertyName)
        {
            return Set(value, null, propertyName);
        }
        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="value">Value of Property</param>
        /// <param name="action">Action to be done if the property has changed</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Whether the property's value has changed</returns>
        protected virtual bool Set<T>(T value, Action action = null, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (PropertyValues.ContainsKey(propertyName))
            {
                if (EqualityComparer<T>.Default.Equals((T)PropertyValues[propertyName], value))
                    return false;
                else
                    PropertyValues[propertyName] = value;
            }
            else
            {
                PropertyValues.Add(propertyName, value);
            }

            OnPropertyChanged(propertyName);

            action?.Invoke();

            return true;
        }

        /// <summary>
        /// Gets the value of a property as an object, created with the <see cref="Set{T}(T, Action, string)"/> Method
        /// </summary>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Value of Property</returns>
        protected Object Get([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (PropertyValues.ContainsKey(propertyName))
            {
                return PropertyValues[propertyName];
            }

            throw new Exception($"Property: ${propertyName} not found");
        }
        /// <summary>
        /// Gets the value of a property created with the <see cref="Set{T}(T, Action, string)"/> Method
        /// if value is not set returns default value if possible else throws PropertyNotSetException
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Value of Property</returns>
        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (PropertyValues.ContainsKey(propertyName))
            {
                return (T)PropertyValues[propertyName];
            }

            return default(T);
        }


        protected Delegate[] GetEvents()
        {
            return PropertyChanged.GetInvocationList();
        }

        public void Dispose()
        {
            Dispose(true);

            if (!Disposed)
            {
                ClearBasePropertyValues();
            }

            Disposed = true;

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispoing) { }

        protected void ClearBasePropertyValues(bool triggerEvents = true)
        {
            if (_PropertyValues != null)
            {
                List<string> properties = _PropertyValues.Keys.ToList();
                _PropertyValues.Clear();

                if (triggerEvents)
                    OnPropertyChanged(properties);
            }

            _PropertyValues = null;
        }
    }
}