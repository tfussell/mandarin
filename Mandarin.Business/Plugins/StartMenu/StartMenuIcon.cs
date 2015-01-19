using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using IWshRuntimeLibrary;
using Mandarin.Business.Core;
using System.Runtime.InteropServices;

namespace Mandarin.Plugins.StartMenu
{
    public class StartMenuIcon : DockItem
    {
        public override IEnumerable<DockItemAction> MenuItems
        {
            get
            {
                return new List<DockItemAction>
                {
                    DockItemAction.CreateNormal("Open Start Menu", OpenStartMenu)
                };
            }
        }

        private readonly Bitmap orb;

        public StartMenuIcon()
        {
            orb = Api.Load();

            Name = "Start Menu";
            Image = orb;
        }

        protected override void OnLeftClick(object sender, EventArgs e)
        {
            OpenStartMenu();
        }

        private static void OpenStartMenu()
        {
            var shell = new WshShell();
            shell.SendKeys("^{ESC}");
        }

        private class Api
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr FindResource(IntPtr hModule, string lpName, uint lpType);
            [DllImport("kernel32.dll")]
            static extern IntPtr FindResource(IntPtr hModule, int lpID, string lpType);
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

            const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

            static internal Bitmap Load()
            {
                IntPtr hMod = LoadLibraryEx(Path.Combine(Environment.SystemDirectory, "twinui.dll"), IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                IntPtr hRes = FindResource(hMod, 11705, "IMAGE");

                uint size = SizeofResource(hMod, hRes);
                IntPtr pt = LoadResource(hMod, hRes);

                Bitmap bmp;
                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);
                using (MemoryStream m = new MemoryStream(bPtr))
                    bmp = (Bitmap)Bitmap.FromStream(m);
                return bmp;
            }
        }
    }
}
