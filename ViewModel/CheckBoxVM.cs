using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BaiscFileExplorer.ViewModel
{
    /// <summary>
    /// View model of Show hidden items checkbox control
    /// </summary>
    public class CheckBoxVM
    {
        #region Property Definitions
        // Checkbox checked bool value, to bind to
        private bool _canShowHiddenItems { get; set; }
        public bool CanShowHiddenItems
        {
            get
            {
                return _canShowHiddenItems;
            }
            set
            {
                _canShowHiddenItems = value;
                RebuildItemCollection();
            }
        }
        #endregion

        #region Functions
        // Function to update Treeview Item collection
        private void RebuildItemCollection()
        {
            AppVM.TreeView.Items = new ObservableCollection<TreeViewItemVM>(TreeViewVM.GetMainDrive().Select(drive => new TreeViewItemVM(drive.Name, TreeViewItemType.Drive)));
        }
        #endregion
    }
}
