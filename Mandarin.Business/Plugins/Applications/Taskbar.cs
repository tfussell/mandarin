using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;
using WinEventHook;
using System.Diagnostics;

namespace Mandarin.Plugins.Applications
{
    internal class Taskbar : IDisposable
    {
        public List<ApplicationDockItem> Items { get; set; }

        public Taskbar()
        {
            var pinned = LoadPinnedItems().ToList();
            var visibleWindowsHandles = LoadTopLevelWindows().ToList();
            Items = new List<ApplicationDockItem>();

            foreach (var item in pinned)
            {
                var entry = DesktopEntryManager.FromFile(item);
                var dockItem = new ApplicationDockItem(entry);
                dockItem.Pinned = true;
                Items.Add(dockItem);
            }

            foreach (var hWnd in visibleWindowsHandles)
            {
                var id = AppUserModelId.Find(hWnd).Where(a => File.Exists(a.DestinationList)).FirstOrDefault();
                bool match = false;

                if (id != null)
                {
                    foreach (var item in Items)
                    {
                        if (item.AppId != null && item.AppId.Id == id.Id)
                        {
                            item.RegisterWindowHandle(hWnd);
                            match = true;
                            break;
                        }
                    }
                }

                if (!match)
                {
                    var dockItem = new ApplicationDockItem(hWnd);
                    //Items.Add(dockItem);
                }
            }
        }

        protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("user32.dll")]
        protected static extern bool IsWindowVisible(IntPtr hWnd);

        public IEnumerable<IntPtr> LoadTopLevelWindows()
        {
            var topLevelWindows = new List<IntPtr>();

            EnumWindows(new EnumWindowsProc((IntPtr hWnd, IntPtr lParams) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    topLevelWindows.Add(hWnd);
                }
                return true;
            }), IntPtr.Zero);

            return topLevelWindows;
        }

        public IEnumerable<string> LoadPinnedItems()
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
