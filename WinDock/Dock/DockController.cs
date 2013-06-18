using System;
using System.Collections.Generic;
using System.Linq;
using WinDock.Configuration;

namespace WinDock.Dock
{
    internal class DockController : IDisposable
    {
        public delegate void AllDocksClosedDelegate();

        public delegate void ButtonClickDelegate();

        public IEnumerable<DockConfiguration> Configuration
        {
            private get { return configuration; }
            set
            {
                if (Equals(configuration, value)) return;
                configuration = value;
                OnConfigurationChanged();
            }
        }

        private IEnumerable<DockConfiguration> configuration;
        private readonly List<Dock> docks;

        public DockController()
        {
            docks = new List<Dock>();
        }

        public void Dispose()
        {
        }

        public event ButtonClickDelegate AboutButtonClick = delegate { };
        public event ButtonClickDelegate ConfigurationButtonClick = delegate { };

        public event AllDocksClosedDelegate AllDocksClosed = delegate { };

        public void Initialize(IEnumerable<DockConfiguration> config)
        {
            Configuration = config;

            foreach (var dockConfig in Configuration)
            {
                AddDock(dockConfig);
            }
        }

        private void AddDock(DockConfiguration config)
        {
            var d = new Dock(config);

            d.AboutButtonClick += () => AboutButtonClick();
            d.ConfigurationButtonClick += () => ConfigurationButtonClick();
            d.Closed += () => DeleteDock(d);

            d.Initialize();
            docks.Add(d);
        }

        private void DeleteDock(Dock dock)
        {
            docks.Remove(dock);

            if (docks.Count == 0)
            {
                AllDocksClosed();
            }
        }

        private void OnConfigurationChanged()
        {
            while (docks.Any())
            {
                docks[0].Close();
                docks.Remove(docks[0]);
            }

            foreach (var config in Configuration)
            {
                AddDock(config);
            }
        }
    }
}