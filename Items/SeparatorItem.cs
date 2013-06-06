using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinDock
{
    class SeparatorItem : DockItem
    {
        public SeparatorItem()
        {
            Image = System.Drawing.Image.FromFile("C:\\Users\\William\\Documents\\Visual Studio 2010\\Projects\\MultiDock\\MultiDock\\Resources\\Icons\\separator.png");
            WithinContainerBounds = true;
        }
    }
}
