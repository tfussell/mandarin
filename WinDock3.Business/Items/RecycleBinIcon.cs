using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using WinDock3.Business.ContextMenu;
using WindowsManagedApi;

namespace WinDock3.Business.Items
{
    public class RecycleBinIcon : DockItem
    {
        private bool IsEmpty
        {
            get { return WindowsManagedApi.Shell32.SHQueryRecycleBinW() == 0; }
        }

        public override IEnumerable<ContextMenuItem> MenuItems
        {
            get
            {
                return new List<ContextMenuItem>
                    {
                        new TextContextMenuItem("Open", OpenInExplorer),
                        new SeparatorContextMenuItem(),
                        new TextContextMenuItem("Empty Trash", Empty)
                    };
            }
        }

        private readonly string recycleBinPath;
        private readonly Bitmap empty;
        private readonly Bitmap full;
        private readonly FileSystemWatcher folderWatcher;

        public RecycleBinIcon()
        {
            full = (Bitmap)User32.ExtractIconW(Path.Combine(Environment.SystemDirectory, "imageres.dll"), 49);
            empty = (Bitmap)User32.ExtractIconW(Path.Combine(Environment.SystemDirectory, "imageres.dll"), 50);

            Name = "Recycle Bin";

            var currentUser = WindowsIdentity.GetCurrent();
            var currentUserSid = currentUser != null ? currentUser.User : null;

            if (currentUserSid != null)
            {
                recycleBinPath = String.Format(Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), String.Format(@"$Recycle.Bin\{0}", currentUserSid.Value)));
                if(Directory.Exists(recycleBinPath))
                    folderWatcher = new FileSystemWatcher {Path = recycleBinPath};
            }

            folderWatcher.EnableRaisingEvents = true;
            folderWatcher.Changed += (sender, args) =>
                {
                    Image = IsEmpty ? empty : full;
                };

            Image = IsEmpty ? empty : full;
        }

        private static void OpenInExplorer()
        {
            Process.Start("explorer.exe", "shell:RecycleBinFolder");
        }

        private void Empty()
        {
            WindowsManagedApi.Shell32.SHEmptyRecycleBinW(false, false, false);
            Image = empty;
        }
    }
}
