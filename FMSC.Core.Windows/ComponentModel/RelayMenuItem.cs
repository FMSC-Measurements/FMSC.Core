﻿using FMSC.Core.ComponentModel;
using FMSC.Core.Windows.ComponentModel.Commands;
using System;

namespace FMSC.Core.Windows.ComponentModel
{
    public class RelayMenuItem : BaseModel
    {
        public String Header
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        
        public bool IsVisible
        {
            get { return Get<bool>(); }
            set { Set(value); }
        }

        public bool IsEnabled
        {
            get { return Get<bool>(); }
            set { Set(value, () => Command?.OnCanExecuteChanged(this, null)); }
        }
        
        public RelayCommand Command
        {
            get { return Get<RelayCommand>(); }
            set { Set(value); }
        }

        public RelayMenuItem(String header, Action<object> execute = null)
        {
            Header = header;
            Command = new RelayCommand(execute, (x) => IsEnabled);
        }
    }
}
