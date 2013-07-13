using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WinDock3.Business.ContextMenu;

namespace WinDock3.Business.Items
{
    public class SeparatorItem : DockItem
    {
        public override IEnumerable<ContextMenuItem> MenuItems
        {
            get { return Enumerable.Empty<ContextMenuItem>(); }
        }

        public SeparatorItem()
        {
            WithinContainerBounds = true;
        }
    }
}
