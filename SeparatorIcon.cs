using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinDock
{
    class SeparatorIcon : DockIcon
    {
        public SeparatorIcon()
        {
            Bitmap = (Bitmap)Image.FromFile("C:\\Users\\William\\Desktop\\iconsFolder\\separator.png");
            Width = Configuration.IconSize / 2;
            Height = (int)(Configuration.IconSize * 1.35);
        }
    }
}
