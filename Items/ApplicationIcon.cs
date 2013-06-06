using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AnotherDirect2DTest
{
    internal class ApplicationIcon : DockItem
    {
        public string DisplayName;
        public string ExecutablePath;

        public ApplicationIcon(string filename)
        {
            ExecutablePath = filename;

            if (IsShortcut(filename))
            {
                DisplayName = Path.GetFileNameWithoutExtension(filename);
                var referent = Shortcut.ParseShortcut(filename);
                if (referent != null)
                    Image = ExtractIconBitmap(referent) ?? ExtractIconBitmap(filename);
            }

            if (Image == null)
            {
                var associatedIcon = Icon.ExtractAssociatedIcon(filename);
                if (associatedIcon != null)
                    Image = associatedIcon.ToBitmap();
            }

            if (Image != null)
            {
                ReflectionImage = CreateReflection(Image);
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            Process.Start(ExecutablePath);
        }

        private static bool IsShortcut(string filename)
        {
            return Path.GetExtension(filename.ToLower()) == ".lnk";
        }

        private static Bitmap ExtractIconBitmap(string path)
        {
            // Try for the biggest icon and then work down in powers of two
            for (var size = 512; size > 16; size /= 2)
            {
                var phicon = new[] { IntPtr.Zero };
                var piconid = new[] { IntPtr.Zero };

                Api.PrivateExtractIcons(path, 0, size, size, phicon, piconid, 1, 0);

                if (phicon[0] != IntPtr.Zero)
                {
                    return Icon.FromHandle(phicon[0]).ToBitmap();
                }
            }

            return null;
        }

        private static class Api
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern UInt32 PrivateExtractIcons(String lpszFile, int nIconIndex, int cxIcon, int cyIcon,
                                                            IntPtr[] phicon, IntPtr[] piconid, UInt32 nIcons,
                                                            UInt32 flags);
        }
    }
}
