using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinDock
{
    class RecycleBinIcon : DockIcon
    {
        public RecycleBinIcon()
        {
            Bitmap = (Bitmap)Image.FromFile("C:\\Users\\William\\Desktop\\iconsFolder\\Trash_Full.png");

            Width = Configuration.IconSize;
            Height = Configuration.IconSize;
        }
    }
}
