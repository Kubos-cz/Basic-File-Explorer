using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiscFileExplorer
{
    /// <summary>
    /// View model of Status Bar Control
    /// </summary>
    public class StatusBarVM : INotifyPropertyChanged
    {
        // Event handler
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Propery Definitions
        // Item count of selected items
        private int _itemCountValue { get; set; } = 0;
        public int ItemCountValue
        {
            get
            {
                return _itemCountValue;
            }
            set
            {
                _itemCountValue = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ItemCountText)));
            }
        }
        // Immediate selected item text value
        public string ItemCountText
        {
            get
            {
                if (_itemCountValue == 0)
                {
                    return $"";
                }
                else if (_itemCountValue == 1)
                {
                    return $"{_itemCountValue} item";
                }
                else
                {
                    return $"{_itemCountValue} items";
                }
            }
            set
            {

            }
        }

        // Item child count
        private int _childItemCount { get; set; } = 0;
        public int ChildItemCount
        {
            get
            {
                return _childItemCount;
            }
            set
            {
                _childItemCount = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ChildItemText)));
            }
        }
        private long _childItemSize { get; set; } = 0;
        // Item size in bytes
        public long ChildItemSize
        {
            get
            {
                return _childItemSize;
            }
            set
            {
                _childItemSize = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ChildItemText)));
            }
        }
        // Item child count & size text value
        public string ChildItemText
        {
            get 
            {
                if (_childItemCount == 0)
                {
                    if (_childItemSize == 0)
                    {
                        return $"Empty Folder";
                    }
                    else
                    {
                        return $"{ChildItemSize} bytes";
                    }
                }
                else if (_childItemCount == 1)
                {
                    return $"{_childItemCount} item, {ChildItemSize} bytes";
                }
                else
                {
                    return $"{_childItemCount} items, {ChildItemSize} bytes";
                }
            }
            set
            {

            }
        }
        #endregion
    }
}
