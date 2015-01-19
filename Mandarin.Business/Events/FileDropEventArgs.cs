using System;

namespace Mandarin.Business.Events
{
    public enum DockDropType
    {
        None,
        File,
        DockItem
    }

    public class FileDropEventArgs : EventArgs
    {
        public int Index;
        public DockDropType DropType;
        public string File;
    }
}
