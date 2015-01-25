using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using Mandarin.Business.Settings;
using Mandarin.Business.Core;
using Mandarin.Business.Events;
using WinEventHook;
using System.Diagnostics;

namespace Mandarin.Plugins.Applications
{
    public class ApplicationIconGroup : DockItemGroup
    {
        private static readonly string PinnedIconsFolder;
        private Hook hook;

        static ApplicationIconGroup()
        {
            PinnedIconsFolder = Path.Combine(ConfigurationController.ApplicationDataFolder, "Pinned");
        }

        private readonly List<ApplicationDockItem> items;

        public ApplicationIconGroup()
        {
            items = new List<ApplicationDockItem>();
            LoadIcons();
        }

        private void LoadIcons()
        {
            // Load application shortcuts from existing folder if possible
            if (Directory.Exists(PinnedIconsFolder) && Directory.GetFiles(PinnedIconsFolder).Where(s => new FileInfo(s).Attributes.HasFlag(FileAttributes.Hidden) == false).Count() > 0)
            {
                foreach (var appIconShortcutFile in Directory.GetFiles(PinnedIconsFolder))
                {
                    var entry = DesktopEntryManager.FromFile(appIconShortcutFile);
                    try
                    {
                        var possibleAppIds = AppUserModelId.Find(entry.TryExec);
                        var appId = possibleAppIds.FirstOrDefault(a => File.Exists(a.DestinationList)) ?? possibleAppIds.First();

                        if (appId != null)
                        {
                            var item = new ApplicationDockItem(entry);
                            items.Add(item);
                            OnItemsChanged(this, ItemsChangedEventArgs<DockItem>.BuildAddedEvents(new List<DockItem> { item }));
                        }
                    }
                    catch { }
                }
            }
            else // No existing items found, try to load icons from the Taskbar
            {
                using (var taskbar = new Taskbar())
                {
                    items.AddRange(taskbar.Items);
                }
            }

            hook = new Hook();
            var timer = new System.Timers.Timer(100);
            timer.Elapsed += (object o, System.Timers.ElapsedEventArgs e) =>
            {
                while (hook.HasCreatedWindow())
                {
                    var hwnd = hook.PopCreatedWindow();
                    var process = Process.GetProcesses().Where(p => p.MainWindowHandle == hwnd).SingleOrDefault();
                    if (process == null) continue;
                    foreach (var item in Items)
                    {
                        if (((ApplicationDockItem)item).DesktopEntry.TryExec == process.MainModule.FileName)
                        {
                            ((ApplicationDockItem)item).RegisterWindowHandle(hwnd);
                            break;
                        }
                    }
                }

                while (hook.HasDestroyedWindow())
                {
                    var hwnd = hook.PopDestroyedWindow();
                    foreach (var item in Items)
                    {
                        if (((ApplicationDockItem)item).HasRegisteredWindowHandle(hwnd))
                        {
                            ((ApplicationDockItem)item).UnregisterWindowHandle(hwnd);
                            break;
                        }
                    }
                }
            };
            timer.Enabled = true;
        }

        public override string Name
        {
            get { return "Applications"; }
        }

        public override IEnumerable<DockItem> Items
        {
            get { return items.Cast<DockItem>(); }
        }
    }
}
