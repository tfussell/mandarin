using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WinDock3.Presentation.Controls.ImageGridRenderers
{
    public interface IImageGridRenderer
    {
        Visual Render(ImageSource sourceImage, double width, double height);
    }
}
