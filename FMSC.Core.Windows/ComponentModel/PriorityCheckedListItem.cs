using FMSC.Core.ComponentModel;
using System;

namespace FMSC.Core.Windows.ComponentModel
{
    public class PriorityCheckedListItem<T> : CheckedListItem<T>
    {
        private bool _IsPriority;
        public bool IsPriority
        {
            get { return _IsPriority; }
            set { SetField(ref _IsPriority, value); }
        }

        public PriorityCheckedListItem() { }

        public PriorityCheckedListItem(T item, bool isChecked = false, bool isPriority = false) : base(item, isChecked)
        {
            IsPriority = isPriority;
        }
    }
}
