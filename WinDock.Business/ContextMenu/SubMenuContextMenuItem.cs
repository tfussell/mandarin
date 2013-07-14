using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinDock.Business.ContextMenu
{
    public class SubMenuContextMenuItem : ContextMenuItem
    {
        public string Text { get; set; }
        public IEnumerable<ContextMenuItem> SubMenu { get; set; }
    }
}
