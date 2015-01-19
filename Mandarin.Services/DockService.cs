using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandarin.Business.Core;
using Mandarin.Services.Interfaces;
using System.IO;
using Mandarin.Business.Settings;

namespace Mandarin.Services
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
                .Select(d => DockConfiguration.FromFile(Path.Combine(d, "dock.json")))
                .DefaultIfEmpty(DockConfiguration.Default);

            var docks = dockConfigurations.Select(config => new Dock(config)).ToList();

            callback(docks, null);
        }

        public void SaveDock(Dock dock, Action<bool> callback)
        {
            throw new NotImplementedException();
        }
    }
}
