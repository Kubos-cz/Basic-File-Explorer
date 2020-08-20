using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BaiscFileExplorer
{
    /// <summary>
    /// View model of TreeView Control
    /// </summary>
    class TreeViewVM : INotifyPropertyChanged
    {
        // Event handler
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Propery Definitions
        // Item collection of the tree view
        private ObservableCollection<TreeViewItemVM> _items { get; set; }
        public ObservableCollection<TreeViewItemVM> Items {
            get 
            {
                return _items;
            } 
            set 
            { 
                _items = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Items)));
            } 
        }

        // Cancellation token
        private static CancellationTokenSource CancellationTokenSource;

        //Currently selected item
        private static TreeViewItemVM _selectedItem = null;

        public static TreeViewItemVM SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    var t = Task.Run(() => GetSelectedItemStatisticsAsync());
                }
            }
        }
        #endregion

        // Constructor
        public TreeViewVM()
        {
            // Get the logical drive
              // set our first collection to be the C drive
            Items = new ObservableCollection<TreeViewItemVM>(GetMainDrive().Select(drive => new TreeViewItemVM(drive.Name, TreeViewItemType.Drive)));

        }

        #region Functions
        // Start calculating size and content of selected item 
        private static async Task GetSelectedItemStatisticsAsync() 
        {
            // Cancel running tasks
            if (CancellationTokenSource != null) { CancellationTokenSource.Cancel(); }
            // Reset cancelation token
            CancellationTokenSource = new CancellationTokenSource();
            // Get content count of selected item
            await _selectedItem.GetContentCountAsync();
            // Get size of selected item
            await _selectedItem.GetitemSizeAsync(CancellationTokenSource.Token);
        }

        // Get logical drives and add only the first one (presumably C:\\)
        public static List<TreeViewItem> GetMainDrive()
        {
            // Get drives from system IO
            var logicalDrives = Directory.GetLogicalDrives();
            // Create a list of logical drive items, add only the first drive found
            var driveItems = new List<TreeViewItem>() {
            (new TreeViewItem { Name = logicalDrives[0], Path = logicalDrives[0], Type = TreeViewItemType.Drive })
        };
            return driveItems;
        }

        #endregion
    }
}
