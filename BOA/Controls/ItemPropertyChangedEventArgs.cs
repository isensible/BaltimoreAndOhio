using System.ComponentModel;

namespace BOA.Controls
{
    public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        readonly object _item;

        public ItemPropertyChangedEventArgs(object item, string propertyName)
            : base(propertyName)
        {
            _item = item;
        }

        public object Item
        {
            get { return _item; }
        }
    }
}