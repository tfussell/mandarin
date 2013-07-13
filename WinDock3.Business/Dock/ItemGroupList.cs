using System.Collections.Generic;
using System.Linq;
using WinDock3.Business.ItemGroups;
using WinDock3.Business.Items;

namespace WinDock3.Business.Dock
{
    internal class ItemGroupList
    {
        private readonly List<DockItemGroup> groups;

        public ItemGroupList()
        {
            groups = new List<DockItemGroup>();
        }

        public IEnumerable<DockItem> AllItems
        {
            get { return groups.SelectMany(i => i.Items); }
        }

        public DockItemGroup GetGroup(int index)
        {
            return groups[index];
        }

        public DockItemGroup GetGroup(string name)
        {
            return groups.Single(g => g.Name == name);
        }

        public void AddGroup(DockItemGroup group)
        {
            groups.Add(group);
        }

        public void InsertGroup(DockItemGroup group, int index)
        {
            groups.Insert(index, group);
        }

        public void RemoveGroup(string name)
        {
            groups.Remove(GetGroup(name));
        }

        public DockItemGroup GetGroupByItemIndex(int index)
        {
            int cummulativeIndex = 0;
            foreach (var itemGroup in groups)
            {
                cummulativeIndex += itemGroup.Items.Count();
                if (cummulativeIndex > index)
                {
                    return itemGroup;
                }
            }
            return groups.Last();
        }
    }
}