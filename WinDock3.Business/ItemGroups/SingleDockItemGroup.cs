using System.Collections.Generic;
using WinDock3.Business.Items;

namespace WinDock3.Business.ItemGroups
{
    public class SingleDockItemGroup<T> : DockItemGroup where T : DockItem, new()
    {
        private readonly DockItem dockItem;

        public SingleDockItemGroup()
        {
            dockItem = new T();
        }

        public override string Name
        {
            get { return dockItem.Name; }
        }

        public override IEnumerable<DockItem> Items
        {
            get { yield return dockItem; }
        }
    }
}
