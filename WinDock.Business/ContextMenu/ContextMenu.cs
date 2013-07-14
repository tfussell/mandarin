using System.Collections.Generic;

namespace WinDock.Business.ContextMenu
{
    public class ContextMenu
    {
        public ContextMenu(IContextMenuProvider subject)
        {
            Subject = subject;
        }

        public IEnumerable<ContextMenuItem> MenuItems
        {
            get { return Subject.MenuItems; }
        }

        public IContextMenuProvider Subject { get; set; }
    }
}
