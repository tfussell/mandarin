using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WinDock.GUI;

namespace WinDock.Items
{
    public class DockItemGroup
    {
		public event NotifyCollectionChangedEventHandler ItemsChanged;
		
		public string Name { get; set; }
		
		public virtual bool Separated {
			get { return false; }
		}
		
		public virtual bool AutoDisable {
			get { return !disposing; }
		}
		
        
		ObservableCollection<DockItem> items;
		public ObservableCollection<DockItem> Items {
			get { return items; }
		    protected set
		    {
		        var intersect = value.Intersect(items);
		        value.Where(v => !intersect.Contains(v)).ToList().ForEach(v => items.Add(v));
                items.Where(v => !intersect.Contains(v)).ToList().ForEach(v => items.Remove(v));
		    }
		}

		protected DockItemGroup ()
		{
			items = new ObservableCollection<DockItem>();
            items.CollectionChanged += ItemsOnCollectionChanged;
		}

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemsChanged(sender, e);
        }

        public bool CanAcceptDrop (string uri)
		{
			return OnCanAcceptDrop(uri);
		}
		
		protected virtual bool OnCanAcceptDrop (string uri)
		{
			return false;
		}
		
		public bool AcceptDrop (string uri, int position)
		{
			DockItem newItem = OnAcceptDrop (uri);
			
			if (newItem != null) 
            {
				newItem.Index = position;
                foreach (var item in Items.Where(adi => adi != newItem && adi.Index >= newItem.Index))
                {
                    item.Index++;
                }
            }
			
			return newItem != null;
		}
		
		protected virtual DockItem OnAcceptDrop(string uri)
		{
			return null;
		}
		
		public virtual bool ItemCanBeRemoved(DockItem item)
		{
			return true;
		}
		
		public virtual bool RemoveItem (DockItem item)
		{
		    if (!ItemCanBeRemoved(item))
		    {
		        return false;
		    }

		    var saved = Items.Where(adi => adi != item).ToArray();
			item.Dispose();
			return true;
		}
		
		public virtual void GetMenuItems(DockItem item, RightClickMenu menu)
		{
		    item.AddRightClickMenuItems(menu);
		}

        #region IDisposable implementation
        bool disposing = false;
		
		public virtual void Dispose ()
		{
			disposing = true;

		    foreach (DockItem adi in Items)
		    {
		        adi.Dispose();
		    }
        }
        #endregion
    }
}
