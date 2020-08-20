using BaiscFileExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiscFileExplorer
{
    /// <summary>
    /// View model of the main window
    /// </summary>
    class AppVM
    {
        #region Property definitions
        // Treeview control
        public static TreeViewVM TreeView { get; set; }
        // Status bar control
        public static StatusBarVM StatusBar { get; set; }
        // Check box control
        public static CheckBoxVM CanShowHiddenItemsCheckBox { get; set; }
        #endregion

        // Constructor
        public AppVM()
        {
            TreeView = new TreeViewVM();
            StatusBar = new StatusBarVM();
            CanShowHiddenItemsCheckBox = new CheckBoxVM();
        }
    }
}
