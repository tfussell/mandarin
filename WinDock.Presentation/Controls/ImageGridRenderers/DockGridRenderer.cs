using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WinDock.Presentation.Controls.ImageGridRenderers
{
    public class DockGridRenderer : IImageGridRenderer
    {
        public Visual Render(ImageSource sourceImage, double width, double height)
        {
            var drawingVisual = new DrawingVisual();
            var bitmapSource = (BitmapSource)sourceImage;

            var drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(0, 0, 50, 150)), new Rect(0, 0, 50, height));
            if (width > 100)
                drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(50, 0, 900, 150)), new Rect(50, 0, width - 100, height));
            if (width > 50)
                drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(950, 0, 50, 150)), new Rect(width - 50, 0, 50, height));

            drawingContext.Close();

            return drawingVisual;
        }
    }
}
