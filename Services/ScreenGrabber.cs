using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WinDock
{
    class ScreenGrabber
    {
        public ScreenGrabber(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Bitmap Capture()
        {
            Bitmap bitmap = new Bitmap(Width, Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(X, Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
