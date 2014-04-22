using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using WinDock.Business.Settings;
using WinDock.Business.Core;
using WinDock.Business.Events;

namespace WinDock.Plugins.Applications
{
    public class ApplicationIconGroup : DockItemGroup
    {
        private static readonly string PinnedIconsFolder;

        static ApplicationIconGroup()
        {
            PinnedIconsFolder = Path.Combine(ConfigurationController.ApplicationDataFolder, "WinDockPinned");
        }

        private readonly OrderedDictionary items;

        public ApplicationIconGroup()
        {
            items = new OrderedDictionary();
            LoadIcons();
        }

        private void LoadIcons()
        {
            // Load application shortcuts from existing folder if possible
            if (Directory.Exists(PinnedIconsFolder))
            {
                foreach (var appIconShortcutFile in Directory.GetFiles(PinnedIconsFolder))
                {
                    var entry = DesktopEntryManager.FromShellLinkFile(appIconShortcutFile);
                    try
                    {
                        var possibleAppIds = AppUserModelId.Find(entry.TryExec);
                        var appId = possibleAppIds.FirstOrDefault(a => File.Exists(a.DestinationList)) ?? possibleAppIds.First();

                        if (appId != null)
                        {
                            var item = new ApplicationDockItem(entry);
                            items.Add(appId, item);
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
                    var referents = taskbar.GetAll().Select(DesktopEntryManager.FromShellLinkFile);
                    var valid = referents.Where(i => i.Type != DesktopEntryType.Invalid).ToList();
                    foreach (var item in valid)
                    {
                        if (item.Type == DesktopEntryType.Application)
                        {
                            var appId = AppUserModelId.Find(item.TryExec).FirstOrDefault();//a => File.Exists(a.DestinationList));
                            if (appId != null)
                            {
                                items.Add(appId.Id, new ApplicationDockItem(item));
                            }
                        }
                        else if (item.Type == DesktopEntryType.Directory)
                        {
                            var appIds = AppUserModelId.FromExplicitAppId("C:\\Windows\\explorer.exe", "Microsoft.Windows.Explorer");
                            var appId = appIds.FirstOrDefault();
                            if (appId != null)
                            {
                                items.Add(appId.Id, new ApplicationDockItem(item));
                            }
                        }
                    }
                }
            }
        }

        private void RegisterWindowHandle(IntPtr hWnd)
        {

        }

        private void UnregisterWindowHandle(IntPtr hWnd)
        {

        }

        public override string Name
        {
            get { return "Applications"; }
        }

        public override IEnumerable<DockItem> Items
        {
            get { return items.Values.Cast<DockItem>(); }
        }
    }
}
