using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Principal;
using S32 = WindowsShellFacade.Shell32;
using WinDock.GUI;

namespace WinDock.Items
{
    class RecycleBinIcon : DockItem
    {
        private bool IsEmpty
        {
            get { return S32.SHQueryRecycleBinW() == 0; }
        }

        private readonly string recycleBinPath;
        private readonly Bitmap empty;
        private readonly Bitmap full;
        private readonly FileSystemWatcher folderWatcher;

        public RecycleBinIcon()
        {
            empty = (Bitmap)Image.FromFile("C:\\Users\\William\\Documents\\Visual Studio 2010\\Projects\\DockResources\\Icons\\Trash_Empty.png");
            full = (Bitmap)Image.FromFile("C:\\Users\\William\\Documents\\Visual Studio 2010\\Projects\\DockResources\\Icons\\Trash_Full.png");

            Width = 50;
            Height = 50;

            Name = "Recycle Bin";

            var currentUser = WindowsIdentity.GetCurrent();
            var currentUserSid = currentUser != null ? currentUser.User : null;

            if (currentUserSid != null)
            {
                recycleBinPath = String.Format(@"C:\$Recycle.Bin\{0}", currentUserSid.Value);
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

        public override void AddRightClickMenuItems(RightClickMenu rightClickMenu)
        {
            rightClickMenu.AddTextItem("Empty Recycle Bin", Empty);
            rightClickMenu.AddTextItem("Open", OpenInExplorer);
        }

        private static void OpenInExplorer()
        {
            Process.Start("explorer.exe", "shell:RecycleBinFolder");
        }

        private void Empty()
        {
            S32.SHEmptyRecycleBinW(false, false, false);
            Image = empty;
        }
    }
}
