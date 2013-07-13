using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WinDock3.Presentation.Controls.ImageGridRenderers
{
    public class ContextMenuGridRenderer : IImageGridRenderer
    {
        public Visual Render(ImageSource sourceImage, double width, double height)
        {
            var drawingVisual = new DrawingVisual();
            var bitmapSource = (BitmapSource)sourceImage;

            if (bitmapSource.PixelHeight != 120 || bitmapSource.PixelWidth != 100)
            {
                throw new Exception("Wrong size");
            }

            var drawingContext = drawingVisual.RenderOpen();

            // Top row
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(0, 0, 24, 24)), new Rect(0, 0, 24, 24));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(24, 0, 52, 24)), new Rect(24, 0, width - 48, 24));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(76, 0, 24, 24)), new Rect(width - 24.325, 0, 24, 24));

            // Middle row
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(0, 24, 24, 52)), new Rect(0, 24, 24, height - 68));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(24, 24, 52, 52)), new Rect(24, 24, width - 48, height - 68));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(76, 24, 24, 52)), new Rect(width - 24.325, 24, 24, height - 68));

            // Bottom row
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(0, 76, 24, 44)), new Rect(0, height - 44, 24, 44));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(24, 76, 11, 44)), new Rect(24, height - 44, 11 + (width - 100) / 2, 44));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(35, 76, 30, 44)), new Rect(35 + (width - 100) / 2, height - 44, 30, 44));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(65, 76, 11, 44)), new Rect(65 + (width - 100) / 2, height - 44, 11 + (width - 100) / 2, 44));
            drawingContext.DrawImage(new CroppedBitmap(bitmapSource, new Int32Rect(76, 76, 24, 44)), new Rect(width - 24.325, height - 44, 24, 44));

            drawingContext.Close();

            return drawingVisual;
        }
    }
}
