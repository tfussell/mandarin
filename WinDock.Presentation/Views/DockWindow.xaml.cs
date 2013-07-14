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
using System.Windows.Shapes;
using WinDock.Business.Core;
using WinDock.PresentationModel.Locators;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WinDock.Presentation.Views
{
    /// <summary>
    /// Interaction logic for DockWindow.xaml
    /// </summary>
    public partial class DockWindow : Window
    {
        #region Dependency properties
        public int ScreenIndex
        {
            get { return (int)GetValue(ScreenIndexProperty); }
            set { SetValue(ScreenIndexProperty, value); }
        }

        public ScreenEdge Edge
        {
            get { return (ScreenEdge)GetValue(EdgeProperty); }
            set { SetValue(EdgeProperty, value); }
        }

        public bool Autohide
        {
            get { return (bool)GetValue(AutohideProperty); }
            set { SetValue(AutohideProperty, value); }
        }

        public bool Reserve
        {
            get { return (bool)GetValue(ReserveProperty); }
            set { SetValue(ReserveProperty, value); }
        }

        public bool BlurEnabled
        {
            get { return (bool)GetValue(BlurEnabledProperty); }
            set { SetValue(BlurEnabledProperty, value); }
        }

        public ImageSource SourceImage
        {
            get { return (ImageSource)GetValue(SourceImageProperty); }
            set { SetValue(SourceImageProperty, value); }
        }
        #endregion

        public static readonly DependencyProperty ScreenIndexProperty;
        public static readonly DependencyProperty EdgeProperty;
        public static readonly DependencyProperty AutohideProperty;
        public static readonly DependencyProperty ReserveProperty;
        public static readonly DependencyProperty BlurEnabledProperty;
        public static readonly DependencyProperty SourceImageProperty;

        private WpfScreen screen;

        static DockWindow()
        {
            ScreenIndexProperty = DependencyProperty.Register("ScreenIndex", typeof(int), typeof(DockWindow), new UIPropertyMetadata(0));
            EdgeProperty = DependencyProperty.Register("Edge", typeof(ScreenEdge), typeof(DockWindow), new UIPropertyMetadata(ScreenEdge.Bottom));
            AutohideProperty = DependencyProperty.Register("Autohide", typeof(bool), typeof(DockWindow), new UIPropertyMetadata(false));
            ReserveProperty = DependencyProperty.Register("Reserve", typeof(bool), typeof(DockWindow), new UIPropertyMetadata(false));
            BlurEnabledProperty = DependencyProperty.Register("BlurEnabled", typeof(bool), typeof(DockWindow), new PropertyMetadata(true, OnBlurChanged));
            SourceImageProperty = DependencyProperty.Register("SourceImage", typeof(ImageSource), typeof(DockWindow), new PropertyMetadata(null, OnSourceImageChanged));
        }

        public DockWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            screen = WpfScreen.Primary;
            Width = screen.WorkingArea.Width;
            Height = 300;
            Left = screen.WorkingArea.Left;
            Top = screen.WorkingArea.Bottom - 300;
            SourceImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\background2.png"));
        }

        private void OnPositionChanged()
        {
            switch ((ScreenEdge)GetValue(EdgeProperty))
            {
                case ScreenEdge.Bottom:
                    ItemPanel.Orientation = Orientation.Horizontal;
                    ItemPanel.UpdateLayout();
                    Left = screen.WorkingArea.Left + (screen.WorkingArea.Width - Width) / 2;
                    Top = screen.WorkingArea.Bottom - Height;
                    break;
                case ScreenEdge.Top:
                    ItemPanel.Orientation = Orientation.Horizontal;
                    ItemPanel.UpdateLayout();
                    Left = screen.WorkingArea.Left + (screen.WorkingArea.Width - Width) / 2;
                    Top = screen.WorkingArea.Top;
                    break;
                case ScreenEdge.Left:
                    ItemPanel.Orientation = Orientation.Vertical;
                    ItemPanel.UpdateLayout();
                    Left = screen.WorkingArea.Left;
                    Top = screen.WorkingArea.Top + (screen.WorkingArea.Height - Height) / 2;
                    break;
                case ScreenEdge.Right:
                    ItemPanel.Orientation = Orientation.Vertical;
                    ItemPanel.UpdateLayout();
                    Left = screen.WorkingArea.Right - Width;
                    Top = screen.WorkingArea.Top + (screen.WorkingArea.Height - Height) / 2;
                    break;
            }
        }

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

        private static void OnBlurChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (DockWindow)sender;
            self.OnBlurChanged(sender, new EventArgs());
        }

        private static void OnSourceImageChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (DockWindow)sender;
            self.OnSourceImageChanged(sender, new EventArgs());
        }

        private void UpdateBackgroundImage()
        {
            if (SourceImage == null || LayoutRoot.RenderSize.Width == 0 || LayoutRoot.RenderSize.Height == 0)
            {
                Background = Brushes.Transparent;
                return;
            }

            var backgroundVisual = Render(SourceImage, ItemPanel.RenderSize.Width, ItemPanel.RenderSize.Height);

            ItemPanel.Background = new VisualBrush(backgroundVisual);
            //UpdateBlur(backgroundVisual);
        }

        private void UpdateBlur(Visual backgroundVisual)
        {
            if (!BlurEnabled) return;

            var backgroundRenderTarget = new RenderTargetBitmap((int)ItemPanel.RenderSize.Width, (int)ItemPanel.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            
            backgroundRenderTarget.Render(backgroundVisual);
            backgroundRenderTarget.Freeze();

            var itemPanelLocation = ItemPanel.PointToScreen(new Point(0, 0));
            var offset = PointFromScreen(itemPanelLocation);

            var blurRegion = BuildRegionFromBitmap(backgroundRenderTarget, offset);
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

        private System.Drawing.Region BuildRegionFromBitmap(BitmapSource bitmap, Point offset)
        {
            var region = new System.Drawing.Region();
            region.MakeEmpty();

            var rectangle = System.Drawing.Rectangle.Empty;
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
                        rectangle.Offset((int)offset.X, (int)offset.Y);
                        region.Union(rectangle);
                    }
                }

                if (inImage)
                {
                    inImage = false;
                    rectangle.Width = (int)bitmap.Width - rectangle.X;
                    rectangle.Offset((int)offset.X, (int)offset.Y);
                    region.Union(rectangle);
                }
            }

            return region;
        }

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
