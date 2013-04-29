using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WinDock
{
    class DockWindow : TransparentWindow
    {
        public DockWindow()
        {
            String roaming_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String taskbar_shortcut_directory = roaming_path + "\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar";

            Icons = LoadIconsFromDirectory(taskbar_shortcut_directory);

            AddIcons();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && Icons != null)
                {
                    foreach (Bitmap bitmap in Icons)
                    {
                        bitmap.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private DockIcon[] LoadIconsFromDirectory(String directory)
        {
            String[] files = Directory.GetFiles(directory);
            DockIcon[] icons = new DockIcon[files.Length];

            int current = 0;

            foreach (String file in files)
            {
                icons[current++] = new DockIcon(file);
            }

            return icons;
        }

        ///<para>Just load a image file and display it on our test form.</para>
        private void AddIcons()
        {
            int icon_size = 56;
            int icon_spacing = 9;
            int canvas_height = 200;
            int canvas_width = 1280;
            int dock_height = 60;
            int dock_side_slope = 30;
            int dock_width = Icons.Length * (icon_size + icon_spacing) + dock_side_slope * 2;
            int gray = 50;
            double alpha = 0.9;

            Bitmap newBitmap = new Bitmap(canvas_width, canvas_height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(newBitmap);

            Point[] points = {new Point((canvas_width - dock_width) / 2 + dock_side_slope, canvas_height - dock_height), 
                             new Point((canvas_width - dock_width) / 2, canvas_height), 
                             new Point((canvas_width + dock_width) / 2, canvas_height), 
                             new Point((canvas_width + dock_width) / 2 - dock_side_slope, canvas_height - dock_height)};

            Color blank_clear = Color.FromArgb(0, 0, 0, 0);
            graphics.Clear(blank_clear);

            Color transparent_gray = Color.FromArgb((int)(alpha * 255), gray, gray, gray);
            SolidBrush polygon_brush = new SolidBrush(transparent_gray);

            graphics.FillPolygon(polygon_brush, points);

            int icon_counter = 0;

            foreach (DockIcon icon in Icons)
            {
                graphics.DrawImage(icon.Bitmap, (canvas_width - dock_width) / 2 + dock_side_slope + (icon_size + icon_spacing) * (icon_counter++), 120, icon_size, icon_size);
            }

            SetBitmap(newBitmap);
        }

        public Array Icons { get; set; }
        private FloatingDockIcon FloatingIcon;
    }
}
