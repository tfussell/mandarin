using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Mandarin.Plugins.Applications
{
    internal class Taskbar : IDisposable
    {
        public List<string> Items { get; set; }

        public Taskbar()
        {
            Items = GetAll().ToList();
        }

        public IEnumerable<string> GetAll()
        {
            var shortcuts = LoadFromRegistry();

            if (shortcuts.Count == 0)
            {
                shortcuts = LoadFromTaskbarPinnedDirectory();
            }

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fullPath = appData + "\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar\\";

            return shortcuts.Select(s => fullPath + s).Where(File.Exists).ToList();
        }

        private static List<String> LoadFromRegistry()
        {
            var registryIconListBytes =
                (byte[])
                Registry.GetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Taskband",
                    "FavoritesResolve", "");
            var result = global::System.Text.Encoding.UTF8.GetString(registryIconListBytes);
            var split = result.Split('\\');

            return
                (from s in split
                 where s.Contains(".lnk")
                 where !s.StartsWith("B�")
                 select s.Substring(0, s.IndexOf(".lnk") + 4)).ToList();
        }

        private static List<String> LoadFromTaskbarPinnedDirectory()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var taskbarPinnedDirectory = appData + "Roaming\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar";

            var shortcuts = new List<String>();

            if (Directory.Exists(taskbarPinnedDirectory))
            {
                var files = Directory.GetFiles(taskbarPinnedDirectory);
                shortcuts.AddRange(shortcuts);
            }

            return shortcuts;
        }

        public void Dispose()
        {
            Items.Clear();
        }
    }
}
