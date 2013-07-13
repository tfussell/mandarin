using System.Collections.Generic;
using WinDock3.Business.Events;
using WinDock3.Business.Items;
using WinDock3.Business.Plugins;
using System;

namespace WinDock3.Business.ItemGroups
{
    public abstract class DockItemGroup
    {
        public delegate void ItemGroupItemsChangedEventHandler(object sender, ItemsChangedEventArgs<DockItem> e);
        public event ItemGroupItemsChangedEventHandler ItemsChanged = delegate { };
		
		public abstract string Name { get; }
        public abstract IEnumerable<DockItem> Items { get; }

        protected void OnItemsChanged(object sender, ItemsChangedEventArgs<DockItem> e)
        {
            ItemsChanged(sender, e);
        }

        public static DockItemGroup FromName(string itemGroupName)
        {
            try
            {
                var plugin = PluginManager.KnownPlugins[itemGroupName];
                var itemGroupConstructor = plugin.ItemGroup.GetConstructor(Type.EmptyTypes);

                return (DockItemGroup)itemGroupConstructor.Invoke(null);
            }
            catch(Exception e)
            {
                throw new Exception("Tried to instantiate unknown item group: " + itemGroupName, e);
            }
        }

        #region IDisposable implementation
        public virtual void Dispose()
        {
            foreach (DockItem item in Items)
            {
                item.Dispose();
            }
        }
        #endregion

    }
}
