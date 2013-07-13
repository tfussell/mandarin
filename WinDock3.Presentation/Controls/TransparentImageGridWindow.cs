using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using WinDock3.Presentation.Controls.ImageGridRenderers;

namespace WinDock3.Presentation.Controls
{
    public enum KnownGrid
    {
        None,
        Dock,
        ToolTip,
        ContextMenu
    }

    public class TransparentImageGridWindow : Window
    {
        #region Dependency properties
        public bool BlurEnabled
        {
            get { return (bool)GetValue(BlurProperty); }
            set { SetValue(BlurProperty, value); }
        }

        public ImageSource SourceImage
        {
            get { return (ImageSource)GetValue(SourceImageProperty); }
            set { SetValue(SourceImageProperty, value); }
        }

        public KnownGrid GridStyle
        {
            get { return (KnownGrid)GetValue(GridStyleProperty); }
            set { SetValue(GridStyleProperty, value); }
        }
        #endregion

        #region Fields
        public static DependencyProperty BlurProperty;
        public static DependencyProperty SourceImageProperty;
        public static DependencyProperty GridStyleProperty;

        private IImageGridRenderer renderer;
        #endregion

        #region Constructors
        static TransparentImageGridWindow()
        {
            BlurProperty = DependencyProperty.Register("Blur", typeof(bool), typeof(TransparentImageGridWindow), new PropertyMetadata(true, OnBlurChanged));
            SourceImageProperty = DependencyProperty.Register("SourceImage", typeof(ImageSource), typeof(TransparentImageGridWindow), new PropertyMetadata(null, OnSourceImageChanged));
            GridStyleProperty = DependencyProperty.Register("GridStyle", typeof(KnownGrid), typeof(TransparentImageGridWindow), new PropertyMetadata(KnownGrid.None, OnGridStyleChanged));
        }

        public TransparentImageGridWindow()
        {
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStyle = System.Windows.WindowStyle.None;
        }
        #endregion

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateBackgroundImage();
        }

        protected virtual void OnBlurChanged(object sender, EventArgs e)
        {
            UpdateBackgroundImage();
        }

        protected virtual void OnSourceImageChanged(object sender, EventArgs e)
        {
            UpdateBackgroundImage();
        }

        protected virtual void OnGridStyleChanged(object sender, EventArgs e)
        {
            switch (GridStyle)
            {
                case KnownGrid.Dock:
                    renderer = new DockGridRenderer();
                    break;
                case KnownGrid.ToolTip:
                    renderer = new DockGridRenderer();
                    break;
                case KnownGrid.ContextMenu:
                    renderer = new ContextMenuGridRenderer();
                    break;
                case KnownGrid.None:
                    renderer = null;
                    break;
            }

            UpdateBackgroundImage();
        }

        private static void OnBlurChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (TransparentImageGridWindow)sender;
            self.OnBlurChanged(sender, new EventArgs());
        }

        private static void OnSourceImageChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (TransparentImageGridWindow)sender;
            self.OnSourceImageChanged(sender, new EventArgs());
        }

        private static void OnGridStyleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (TransparentImageGridWindow)sender;
            self.OnGridStyleChanged(sender, new EventArgs());
        }

        private void UpdateBackgroundImage()
        {
            if (SourceImage == null || RenderSize.Width == 0 || RenderSize.Height == 0)
            {
                Background = Brushes.Transparent;
                return;
            }

            var backgroundVisual = renderer.Render(SourceImage, RenderSize.Width, RenderSize.Height);

            Background = new VisualBrush(backgroundVisual);
            UpdateBlur(backgroundVisual);
        }

        private void UpdateBlur(Visual backgroundVisual)
        {
            if (!BlurEnabled) return;

            var backgroundRenderTarget = new RenderTargetBitmap((int)RenderSize.Width, (int)RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            
            backgroundRenderTarget.Render(backgroundVisual);
            backgroundRenderTarget.Freeze();

            var blurRegion = BuildRegionFromBitmap(backgroundRenderTarget);
            var handle = new WindowInteropHelper(this).Handle;
            var blurBehind = new Api.DWM_BLURBEHIND
            {
                fEnable = true,
                fTransitionOnMaximized = true,
                dwFlags = Api.DWM_BB.BlurRegion | Api.DWM_BB.Enable | Api.DWM_BB.TransitionMaximized,
                hRgnBlur = blurRegion.GetHrgn(System.Drawing.Graphics.FromHwnd(handle))
            };

            Api.DwmEnableBlurBehindWindow(handle, ref blurBehind);
        }

        private System.Drawing.Region BuildRegionFromBitmap(BitmapSource bitmap)
        {
            var region= new System.Drawing.Region();
            region.MakeEmpty();

            var rectangle= new System.Drawing.Rectangle(0,0,0,0);
            bool inImage=false;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var croppedPixel = new CroppedBitmap(bitmap, new Int32Rect(x, y, 1, 1));
                    byte[] pixel = new byte[bitmap.Format.BitsPerPixel / 8];
                    croppedPixel.CopyPixels(pixel, bitmap.Format.BitsPerPixel / 8, 0);
                    var pixelAlpha = pixel[0];

                    if (!inImage && pixelAlpha > 0)
                    {
                        inImage = true;
                        rectangle.X = x;
                        rectangle.Y = y;
                        rectangle.Height = 1;
                    }
                    else if (inImage && pixelAlpha == 0)
                    {
                        inImage = false;
                        rectangle.Width = x - rectangle.X;
                        region.Union(rectangle);
                    }
                }

                if (inImage)
                {
                    inImage = false;
                    rectangle.Width = (int)bitmap.Width - rectangle.X;
                    region.Union(rectangle);
                }
            }

            return region;
        }

        private static class Api
        {
            [Flags]
            public enum DWM_BB
            {
                Enable = 1,
                BlurRegion = 2,
                TransitionMaximized = 4
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct DWM_BLURBEHIND
            {
                public DWM_BB dwFlags;
                public bool fEnable;
                public IntPtr hRgnBlur;
                public bool fTransitionOnMaximized;

                public DWM_BLURBEHIND(bool enabled)
                {
                    fEnable = enabled;
                    hRgnBlur = IntPtr.Zero;
                    fTransitionOnMaximized = false;
                    dwFlags = DWM_BB.Enable;
                }
            }

            [DllImport("dwmapi.dll")]
            public static extern void DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND blurBehind);
        }
    }
}
