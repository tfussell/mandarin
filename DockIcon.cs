using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace WinDock
{
    class DockIcon
    {
        Process process;
        public bool Running {
            get { return process != null && !process.HasExited; }
        }

        public DockIcon(DockIcon cpy)
        {
            Path = cpy.Path;
            Bitmap = cpy.Bitmap;
            ReflectionBitmap = cpy.ReflectionBitmap;
            process = null;
            DisplayName = cpy.DisplayName;
        }

        public DockIcon(String path)
        {
            Path = path;
            String executable_path = path;

            if (IsShortcut(path))
            {
                DisplayName = System.IO.Path.GetFileNameWithoutExtension(path);
                executable_path = ExtractExecutableFromShortcut(path);
            }

            Bitmap = ExtractIconBitmap(executable_path);

            if (Bitmap == null)
            {
                Bitmap = Icon.ExtractAssociatedIcon(executable_path).ToBitmap();
            }

            ReflectionBitmap = CreateReflection(Bitmap);

            process = null;
        }

        Bitmap CreateReflection(Bitmap bitmap)
        {
            Bitmap reflection = new Bitmap(bitmap.Width, bitmap.Height);

            using (Graphics graphics = Graphics.FromImage(reflection))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = 0.3F;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                graphics.DrawImage(bitmap, new Rectangle(0, 0, reflection.Width, reflection.Width), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
            }

            reflection.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return reflection;
        }

        public String Path { get; set; }
        public String DisplayName { get; set; }
        public Bitmap Bitmap { get; private set; }
        public Bitmap ReflectionBitmap { get; private set; }
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

        public void Launch()
        {
            process = Process.Start(Path);
        }
    }
}
