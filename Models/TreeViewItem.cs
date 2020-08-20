using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiscFileExplorer
{
    public class TreeViewItem
    {
        public TreeViewItemType Type { get; set; }
        public string Name { get { return this.Type == TreeViewItemType.Drive ? this.Path : GenerateItemName(this.Path); } set { } }
        public string Path { get; set; }
        private string GenerateItemName(string path)
        {
            // find the last '\' in path and create substring
            return path.Substring(Path.LastIndexOf('\\') + 1);
        }
    }
}
