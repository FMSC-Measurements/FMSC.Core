﻿using FMSC.Core.ComponentModel;
using System;
using System.Windows.Input;

namespace FMSC.Core.Windows.ComponentModel
{
    public class BindableMenuItem : BaseModel
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
            set { Set(value); }
        }
        
        public ICommand Command
        {
            get { return Get<ICommand>(); }
            set { Set(value); }
        }


        public BindableMenuItem(String header, ICommand command = null)
        {
            Header = header;
            Command = command;
        }
    }
}
