using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface INotifyDataErrorInfo
    {
        bool HasErrors { get; }

        IEnumerable<string> GetErrors(String propertyName);

        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }

    public class DataErrorsChangedEventArgs : EventArgs
    {
        public string PropertyName { get; private set; }

        public DataErrorsChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
