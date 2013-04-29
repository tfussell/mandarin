using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using IWshRuntimeLibrary;

namespace WinDock
{
    class DockIcon
    {
        public DockIcon(String path)
        {
            Path = path;
            String executable_path = path;

            if (IsShortcut(path))
            {
                executable_path = ExtractExecutableFromShortcut(path);
            }

            Bitmap = ExtractIconBitmap(executable_path);

            if (Bitmap == null)
            {
                Bitmap = Icon.ExtractAssociatedIcon(executable_path).ToBitmap();
            }
        }

        public String Path { get; set; }
        public String DisplayName { get; set; }
        public Bitmap Bitmap { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private bool IsShortcut(String executable_path)
        {
            return executable_path.ToLower().EndsWith(".lnk");
        }

        private String ExtractExecutableFromShortcut(String executable_path)
        {
            WshShell WShell = new WshShell();
            IWshShortcut link = (IWshShortcut)WShell.CreateShortcut(executable_path);

            return link.TargetPath;
        }

        private Bitmap ExtractIconBitmap(string path)
        {
            // Try for the biggest icon and then work down
            for (int size = 512; size > 16; size /= 2)
            {
                IntPtr[] phicon = new IntPtr[] { IntPtr.Zero };
                IntPtr[] piconid = new IntPtr[] { IntPtr.Zero };

                Win32.PrivateExtractIcons(path, 0, size, size, phicon, piconid, 1, 0);

                if (phicon[0] != IntPtr.Zero)
                {
                    return System.Drawing.Icon.FromHandle(phicon[0]).ToBitmap();
                }
            }

            return null;
        }
    }
}
