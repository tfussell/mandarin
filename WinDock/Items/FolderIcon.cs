using System;
using System.Collections.Generic;
using WinDock.GUI;

namespace WinDock.Items
{
    class FolderIcon : DockItem
    {
        public String Directory { get; private set; }
        public List<String> Contents { get; private set; }

        public FolderIcon(string directory)
        {
            Directory = directory;

            Contents = new List<String>();
            Contents.AddRange(System.IO.Directory.GetFiles(directory));
        }
    }
}
