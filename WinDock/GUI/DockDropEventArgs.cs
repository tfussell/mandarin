using System;

namespace WinDock.GUI
{
    public enum DockDropType
    {
        None,
        File,
        DockItem
    }

    public class DockDropEventArgs : EventArgs
    {
        public int Index;
        public DockDropType DropType;
        public string File;
    }
}
