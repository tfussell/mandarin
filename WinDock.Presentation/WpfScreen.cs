using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace WinDock.Presentation
{
    public class WpfScreen
    {
        public static List<WpfScreen> AllScreens
        {
            get
            {
                return Screen.AllScreens.Select(s => new WpfScreen(s)).ToList();
            }
        }

        public static WpfScreen GetScreenFrom(Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            var screen = Screen.FromHandle(windowInteropHelper.Handle);
            var wpfScreen = new WpfScreen(screen);

            return wpfScreen;
        }

        public static WpfScreen GetScreenFrom(System.Windows.Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            var drawingPoint = new System.Drawing.Point(x, y);
            var screen = Screen.FromPoint(drawingPoint);
            var wpfScreen = new WpfScreen(screen);

            return wpfScreen;
        }

        public static WpfScreen Primary
        {
            get { return new WpfScreen(Screen.PrimaryScreen); }
        }

        private readonly Screen screen;

        internal WpfScreen(Screen screen)
        {
            this.screen = screen;
        }

        public Rect DeviceBounds
        {
            get { return GetRect(screen.Bounds); }
        }

        public Rect WorkingArea
        {
            get { return GetRect(screen.WorkingArea); }
        }

        private Rect GetRect(Rectangle value)
        {
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return screen.Primary; }
        }

        public string DeviceName
        {
            get { return screen.DeviceName; }
        }
    }
}
