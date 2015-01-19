using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Mandarin.Business.Settings;
using Mandarin.Business.Events;

namespace Mandarin.Business.Core
{
    public enum ScreenEdge
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public class Dock : INotifyPropertyChanged, IDisposable
    {
        #region Events
        public event EventHandler Closed = delegate { };
        public event EventHandler AboutButtonClick = delegate { };
        public event EventHandler ConfigurationButtonClick = delegate { };
        public event EventHandler<ItemsChangedEventArgs<DockItem>> ItemsChanged = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion

        #region Public properties
        public DockConfiguration Configuration
        {
            get { return config; }
        }

        public IEnumerable<DockItem> AllItems
        {
            get { return itemGroups.AllItems; }
        }

        public string SeparatorImage
        {
            get { return "separator.png"; }
        }
        #endregion

        private readonly DockConfiguration config;
        private readonly ItemGroupList itemGroups;

        public Dock(DockConfiguration config)
        {
            this.config = config;
            config.PropertyChanged += ConfigOnPropertyChanged;

            itemGroups = new ItemGroupList();
            Initialize();
        }

        public void OnClosed()
        {
            config.Save();
            Closed.Invoke(null, new EventArgs());
        }

        public void Initialize()
        {
            foreach (var name in config.ItemGroups)
            {
                var group = DockItemGroup.FromName(name);
                AddGroup(group);
            }
        }

        private void GroupOnItemsChanged(object sender, ItemsChangedEventArgs<DockItem> e)
        {
            ItemsChanged(sender, e);
        }
        
        private void AddGroup(DockItemGroup group)
        {
            itemGroups.AddGroup(group);

            if(group != null)
                group.ItemsChanged += GroupOnItemsChanged;
        }
        
        private void ConfigOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Autohide")
            {
                PropertyChanged(sender, e);
            }
        }

        public void Dispose()
        {
            
        }
    }
}