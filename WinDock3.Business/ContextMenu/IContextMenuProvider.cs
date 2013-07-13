using System.Collections.Generic;
using System.Drawing;

namespace WinDock3.Business.ContextMenu
{
    public interface IContextMenuProvider
    {
        IEnumerable<ContextMenuItem> MenuItems { get; }
        Rectangle Bounds { get; }
    }
}
