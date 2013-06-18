using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WinDock.Services;

namespace WinDock.Items
{
    class ApplicationIconGroup : DockItemGroup
    {
        private string pinnedIconsFolder = @"C:\Users\William\Desktop\WinDockPinned";

        public ApplicationIconGroup()
        {
            if (Directory.Exists(pinnedIconsFolder) && Directory.GetFiles(pinnedIconsFolder).Any())
            {
                foreach (var appIconShortcutFile in Directory.GetFiles(pinnedIconsFolder, "*.dlnk"))
                {
                    
                }
            }
            else
            {
                using (var taskbar = new TaskbarService())
                {
                    var shortcuts = taskbar.GetAll().Where(i => Path.GetFileNameWithoutExtension(i) == ".lnk");
                    var referents = shortcuts.Select(s => new Shortcut(s));
                    foreach (var item in referents)
                    {
                        
                    }
                }
            }
        }
    }
}
