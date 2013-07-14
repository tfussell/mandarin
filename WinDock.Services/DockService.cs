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
            var dockDirectories = Directory.EnumerateDirectories(Path.Combine(Paths.ResourceDirectory, "Docks"));
            var dockConfigurations = dockDirectories.Where(DockDirectoryIsValid).Select(d => new DockConfiguration()).ToList();

            if (dockConfigurations.Any())
            {
                var docks = dockConfigurations.Select(config => new Dock(config));
                callback(docks, null);
            }
            else
            {
                callback(null, new Exception("No valid dock configurations found in directory: " + Paths.ResourceDirectory));
            }
        }

        public void SaveDock(Dock dock, Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        private bool DockDirectoryIsValid(string directory)
        {
            return File.Exists(Path.Combine(directory, "dock.json"));
        }
    }
}
