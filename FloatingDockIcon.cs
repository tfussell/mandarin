using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WinDock
{
    class FloatingDockIcon : TransparentWindow
    {
        public Bitmap Bitmap { get; set; }
        public Point MouseOffset { get; set; }

        public FloatingDockIcon()
        {

        }
    }
}
