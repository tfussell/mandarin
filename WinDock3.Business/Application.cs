using System;
using System.Collections.Generic;
using System.Linq;
using WinDock3.Business.Settings;

namespace WinDock3.Business
{
    public class Application : IDisposable
    {
        public event Action<object, Dock.Dock> DockAdded = delegate { };
        public event Action<object, Dock.Dock> DockRemoved = delegate { }; 

        public IEnumerable<Dock.Dock> Docks
        {
            get { return docks; }
        }
        
        private List<Dock.Dock> docks;
        private Profile activeProfile;
        private ConfigurationController configuration;

        public Application()
        {
            docks = new List<Dock.Dock>();

            configuration = new ConfigurationController();
            activeProfile = configuration.ActiveProfile;

            foreach (var dockConfig in activeProfile.Docks)
            {
                var dock = new Dock.Dock(dockConfig.Value);
                AddDock(dock);
            }

            configuration.ActiveProfileChanged += ConfigurationOnActiveProfileChanged;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            SaveProfile();
        }

        private void SaveProfile()
        {
            
        }

        private void ConfigurationOnActiveProfileChanged(object sender, Profile profile)
        {
            activeProfile = profile;
            while (docks.Any())
            {
                RemoveDock(docks.First());
            }
            foreach (var dock in profile.Docks)
            {
                AddDock(new Dock.Dock(dock.Value));
            }
        }

        private void RemoveDock(Dock.Dock dock)
        {
            if (dock != null && docks.Contains(dock))
            {
                docks.Remove(dock);
                DockRemoved(this, dock);
            }
        }

        private void AddDock(Dock.Dock dock)
        {
            if (dock != null && !docks.Contains(dock))
            {
                docks.Add(dock);
                DockAdded(this, dock);
            }
        }

        public void Dispose()
        {
        }
    }
}