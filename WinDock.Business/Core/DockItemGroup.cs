using System;
using System.Collections.Generic;
using WinDock.Business.Core;
using WinDock.Business.Events;
using WinDock.Business.Plugins;

namespace WinDock.Business.Core
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
                if (itemGroupName == null) return null;

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
