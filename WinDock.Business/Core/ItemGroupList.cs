using System.Collections.Generic;
using System.Linq;

namespace WinDock.Business.Core
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
            get 
            {
                foreach (var group in groups)
                {
                    if (group == null)
                    {
                        yield return null;
                        continue;
                    }

                    foreach (var item in group.Items)
                    {
                        yield return item;
                    }
                }
            }
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