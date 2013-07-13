using System.Collections.Generic;
using WinDock3.Business.Events;
using WinDock3.Business.ItemGroups;
using WinDock3.Business.Items;
using WindowsManagedApi;

namespace WinDock3.Business.System
{
    public class NotificationArea
    {
        public event DockItemGroup.ItemGroupItemsChangedEventHandler ItemsChanged = delegate { }; 

        public IEnumerable<NotifyIconManaged> Items
        {
            get { return systemNotificationArea.Icons; }
        }

        private readonly NotificationAreaManaged systemNotificationArea;

        public NotificationArea()
        {
            systemNotificationArea = new NotificationAreaManaged();
        }

        protected virtual void OnItemsChanged()
        {
            ItemsChanged(this, new ItemsChangedEventArgs<DockItem>());
        }
    }
}