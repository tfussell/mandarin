using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinDock.Business.Core;
using WinDock.Services.Interfaces;
using System.IO;
using WinDock.Business.Settings;

namespace WinDock.Services
{
    public class DockService : IDockService
    {
        public void GetDocks(Action<IEnumerable<Dock>, Exception> callback)
        {
            if (!Directory.Exists(Paths.Docks))
            {
                Directory.CreateDirectory(Paths.Docks);
            }

            var dockConfigurations = Directory.EnumerateDirectories(Paths.Docks)
                .Where(d => File.Exists(Path.Combine(d, "dock.json")))
                .Select(d => new DockConfiguration())
                .DefaultIfEmpty(DockConfiguration.Default);

            var docks = dockConfigurations.Select(config => new Dock(config));

            callback(docks, null);
        }

        public void SaveDock(Dock dock, Action<bool> callback)
        {
            throw new NotImplementedException();
        }
    }
}
