using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.Drawing.Imaging;
using System.IO;

namespace WinDock
{
    class ApplicationIcon : DockIcon
    {
        Process process;
        public bool Running
        {
            get { return process != null && !process.HasExited; }
        }
        public String ExecutablePath { get; set; }
        private static Bitmap runningIndicatorBitmap;

        static ApplicationIcon()
        {
            runningIndicatorBitmap = (Bitmap)Image.FromFile(Configuration.IconsFolder + Path.DirectorySeparatorChar + Configuration.RunningIndicatorFilename);
        }

        public ApplicationIcon(String path)
        {
            ExecutablePath = path;

            if (IsShortcut(path))
            {
                DisplayName = System.IO.Path.GetFileNameWithoutExtension(path);
                ExecutablePath = ExtractExecutableFromShortcut(path);
            }

            Bitmap = CreateReflection(ExtractIconBitmap(ExecutablePath));

            if (Bitmap == null)
            {
                Bitmap = Icon.ExtractAssociatedIcon(ExecutablePath).ToBitmap();
            }

            process = null;

            Width = Configuration.IconSize;
            Height = Configuration.IconSize * 2;
        }

        public override void OnClick()
        {
            base.OnClick();

            Launch();
        }

        public override void Paint(Graphics graphics)
        {
            base.Paint(graphics);

            if (Running)
                graphics.DrawImage(runningIndicatorBitmap, X + 17, Y + 51, 10, 10);
        }

        Bitmap CreateReflection(Bitmap bitmap)
        {
            Bitmap reflection = new Bitmap(bitmap.Width, bitmap.Height * 2);

            using (Graphics graphics = Graphics.FromImage(reflection))
            {
                graphics.DrawImage(bitmap, new Point(0, 0));

                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = 0.3F;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                graphics.DrawImage(bitmap, new Rectangle(0, bitmap.Height, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
            }

            return reflection;
        }

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
            if(process == null || process.HasExited)
                process = Process.Start(ExecutablePath);
        }
    }
}
