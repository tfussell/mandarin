using System;
using System.Collections.Generic;
using WinDock.Configuration;
using WinDock.GUI;
using WinDock.Items;
using WinDock.Services;

namespace WinDock.Dock
{
    public enum ScreenEdge
    {
        Top,
        Right,
        Bottom,
        Left
    }

    internal class Dock : IDisposable
    {
        public delegate void ButtonClickedDelegate();

        public delegate void ClosedDelegate();

        private readonly DockConfiguration config;
        private readonly List<DockItem> tiles;
        private readonly DockWindow window;

        public Dock(DockConfiguration config)
        {
            this.config = config;

            config.ConfigurationChanged += (p, v) =>
                {
                    switch (p)
                    {
                        case "Edge":
                            window.Edge = (ScreenEdge) v;
                            break;
                        case "ScreenIndex":
                            window.ScreenIndex = (int) v;
                            break;
                    }
                };

            window = new DockWindow
                {
                    Edge = config.Edge,
                    ScreenIndex = config.ScreenIndex,
                    Autohide = false,
                    Reserve = config.Reserve
                };

            tiles = new List<DockItem>();
        }

        public void Close()
        {
            window.Close();
        }

        public void Dispose()
        {
            window.Close();
        }

        public event ClosedDelegate Closed = delegate { };

        public event ButtonClickedDelegate AboutButtonClick = delegate { };
        public event ButtonClickedDelegate ConfigurationButtonClick = delegate { };

        public void Initialize()
        {
            window.Show();
        }
    }
}