using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaiscFileExplorer
{
    /// <summary>
    /// View model of the tree view item
    /// </summary>

    public class TreeViewItemVM : INotifyPropertyChanged
    {
        // Event handler
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Propery Definitions
        // Item type
        public TreeViewItemType Type { get; set; }
        // Name of this item
        public string Name { get { return this.Type == TreeViewItemType.Drive ? this.Path : CreateItemName(this.Path); } set { } }
        // Path to this item
        public string Path { get; set; }
        // Icon of this item
        public string ImageUri => Type == TreeViewItemType.Drive ? "Images/drive.png" : (Type == TreeViewItemType.File ? "Images/file.png" : (IsExpanded ? "Images/foldero.png" : "Images/folderc.png"));
        // Whether this item can expand
        public bool CanExpand { get { return this.Type != TreeViewItemType.File; } }
        // If the item is expanded in the view
        public bool IsExpanded
        {
            get
            {
                return this.Content?.Count(f => f != null) > 0;
            }
            set
            {
                // Unexpanded item was clicked
                if (value == true)
                    // Get content of this item
                    Expand();
                // Expanded item was clicked
                else
                    this.CreateEmptyCollection();
            }
        }

        // Whether this item is selected in UI
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
                if (_isSelected)
                {
                    TreeViewVM.SelectedItem = this;
                }
            }
        }
        // Child content of this item
        public ObservableCollection<TreeViewItemVM> Content { get; set; }
        // Status Bar pointer
        private StatusBarVM StatusBar { get { return AppVM.StatusBar; } set { } }
        #endregion

        // Constructor
        public TreeViewItemVM(string path, TreeViewItemType type)
        {
            // Set path and type, assign status bar
            this.Path = path;
            this.Type = type;
            // create empty collection if item is not a file
            CreateEmptyCollection();
        }

        #region Functions
        // Get child content of this item
        private void Expand()
        {
            // check type for file, which cannot expand
            if (!CanExpand)
            {
                return;
            }
            else
            {
                // Get content of this item
                var children = GetDirectoryContent(this.Path);
                this.Content = new ObservableCollection<TreeViewItemVM>(
                                    children.Select(content => new TreeViewItemVM(content.Path, content.Type)));
                // invoke UI change
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Content)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ImageUri)));
            }
        }
        // Create empty collection (for directories)
        private void CreateEmptyCollection()
        {
            // clear content of this item
            if (this.Type != TreeViewItemType.File)
            {
                // Add null item to display expand icon in treeview to indicate that parent can expand
                this.Content = new ObservableCollection<TreeViewItemVM>() { (null) };
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ImageUri)));
            }
        }
        // Count the number of children of this item
        public async Task GetContentCountAsync()
        {
            // Reset the status bar counter
            StatusBar.ItemCountValue = 0;
            try
            {
                // Check if we are expanded
                if (this.CanExpand & this.IsExpanded)
                {
                    // Return the size of content collection, no need to calculate
                    StatusBar.ItemCountValue = Content.Count;
                }
                else if (this.CanExpand & !this.IsExpanded)
                {
                    // Calculate item count (content collection is empty)
                    StatusBar.ItemCountValue += await Task.Run(() => GetChildDirectories(new DirectoryInfo(this.Path)).Count);
                    StatusBar.ItemCountValue += await Task.Run(() => GetChildFiles(new DirectoryInfo(this.Path)).Count);
                }
                else
                {
                    // This item is of type File therefore return 1
                    StatusBar.ItemCountValue = 1;
                }
            }
            catch
            {
                // Something went wrong :/
            }
        }
        // Get the size of this item
        public async Task GetitemSizeAsync(CancellationToken cancellationToken)
        {
            // Reset status bar counters
            StatusBar.ChildItemCount = 0;
            StatusBar.ChildItemSize = 0;
            try
            {
                // Check the type of this item
                if (this.Type == TreeViewItemType.File)
                {
                    // This is a file, set item count to 1 and calculate file size
                    FileInfo fi = new FileInfo(this.Path);
                    StatusBar.ChildItemSize = fi.Length;
                }
                else
                {
                    // This is a folder
                    // Get count and size of all child items of this directory
                    StatusBar.ChildItemSize += await Task.Run(() => GetDirectorySize(new DirectoryInfo(this.Path)));
                    StatusBar.ChildItemCount += await Task.Run(() => GetChildFiles(new DirectoryInfo(this.Path)).Count);
                    // Get all subdirectories (recursively) of this directory
                    List<string> subdirectories = await Task.Run(() => GetChildDirectoriesRecursively(this.Path, cancellationToken));
                    // For each subdirectory
                    foreach (string subdirectory in subdirectories)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        try
                        {
                            // For each subdirectory, get the size and files count
                            StatusBar.ChildItemSize += await Task.Run(() => GetDirectorySize(new DirectoryInfo(subdirectory)));
                            StatusBar.ChildItemCount += await Task.Run(() => GetChildFiles(new DirectoryInfo(subdirectory)).Count);
                        }
                        catch
                        { 

                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // This task has been canceled
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Access"))
                {
                    // The user running this app has no access to some file in this folder
                }
                else
                {
                    // Something went wrong :/
                }
            }
        }
        // Get subdirectories in a directory
        private List<DirectoryInfo> GetChildDirectories(DirectoryInfo directory)
        {
            List<DirectoryInfo> subdirectories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();
            if (AppVM.CanShowHiddenItemsCheckBox.CanShowHiddenItems)
            {
                return subdirectories;
            }
            else
            {
                return subdirectories.Where(folder => folder.Attributes.HasFlag(FileAttributes.Hidden) == false).ToList() ;
            }
        }
        // Get file in a directory
        private List<FileInfo> GetChildFiles(DirectoryInfo directory)
        {
            try
            {
                List<FileInfo> files = directory.GetFiles("*", SearchOption.TopDirectoryOnly).ToList();
                if (AppVM.CanShowHiddenItemsCheckBox.CanShowHiddenItems)
                {
                    return files;
                }
                else
                {
                    return files.Where(file => file.Attributes.HasFlag(FileAttributes.Hidden) == false).ToList<FileInfo>();
                }
            }
            catch
            {
                return new List<FileInfo>();
            }
        }
        // Get the size of a single directory
        private long GetDirectorySize(DirectoryInfo directory)
        {
            long size = 0;
            try
            {
                size = directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Sum(file => file.Length);
            }
            catch (Exception ex)
            {

            }
            return size;
        }
        // Get all directories (recursively) included in an item
        private List<string> GetChildDirectoriesRecursively(string directory, CancellationToken cancellationToken)
        {
            // Create a new list of directories to return
            List<string> subDirectories = new List<string>();
            try
            {
                // Add all directories of our initial directory to the list 
                subDirectories.AddRange(Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly));
                // For each directory on our list
                foreach (string subDirectory in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                {
                    // Check if cancelation is requested 
                    cancellationToken.ThrowIfCancellationRequested();
                    // Append child count by 1
                    StatusBar.ChildItemCount += 1;
                    try
                    {
                        // Overload this method, add all recursive subdirectories to our list
                        subDirectories.AddRange(GetChildDirectoriesRecursively(subDirectory, cancellationToken));
                    }
                    catch
                    {
                        // Something went wrong, skip this folder
                    }
                }
            }
            catch(OperationCanceledException)
            {
                // This task has been canceled
            }
            // Return list of subdirectories
            return subDirectories;
        }
        // Get content of a directory from system I/O
        public List<TreeViewItem> GetDirectoryContent(string parent)
        {
            try
            {
                List<string> Directories = Directory.GetDirectories(parent).ToList<String>();
                if (!AppVM.CanShowHiddenItemsCheckBox.CanShowHiddenItems)
                {
                    Directories = Directories.Where(directory => new DirectoryInfo(directory).Attributes.HasFlag(FileAttributes.Hidden) == false).ToList<String>();
                }
                var ContentItems = new List<TreeViewItem>();
                ContentItems.AddRange(Directories.Select(directory => new TreeViewItem
                {
                    Path = directory,
                    Type = TreeViewItemType.Folder
                }
                ));

                var Files = Directory.GetFiles(parent).ToList<String>();
                if (!AppVM.CanShowHiddenItemsCheckBox.CanShowHiddenItems)
                {
                    Files = Files.Where(file => new FileInfo(file).Attributes.HasFlag(FileAttributes.Hidden) == false).ToList<String>();
                }
                ContentItems.AddRange(Files.Select(file => new TreeViewItem
                {
                    Path = file,
                    Type = TreeViewItemType.File
                }
));
                return ContentItems;
            }
            catch (Exception e)
            {
                return new List<TreeViewItem>();
            }
        }
        // Generate human friendly name of this Item
        private string CreateItemName(string path)
        {
            // find the last '\' in path and create substring
            return path.Substring(Path.LastIndexOf('\\') + 1);
        }
        #endregion
    }
}
