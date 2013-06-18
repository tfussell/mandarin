using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinDock.Dock;
using WinDock.Items;

namespace WinDock.GUI
{
    internal class LayoutManager
    {
        public ScreenEdge Edge { get; set; }

        public LayoutManager(ScreenEdge edge)
        {
            Edge = edge;
        }

        public Size PerformLayout(Size canvasSize, int baselineHeight, int dockHeight, int iconSize, IEnumerable<DockItem> items)
        {
            switch (Edge)
            {
                case ScreenEdge.Bottom:
                    return PerformLayoutBottom(canvasSize, baselineHeight, dockHeight, iconSize, items);
                case ScreenEdge.Top:
                    return PerformLayoutTop(canvasSize, baselineHeight, dockHeight, iconSize, items);
                case ScreenEdge.Left:
                    return PerformLayoutLeft(canvasSize, baselineHeight, dockHeight, iconSize, items);
                case ScreenEdge.Right:
                    return PerformLayoutRight(canvasSize, baselineHeight, dockHeight, iconSize, items);
                default:
                    throw new Exception("Invalid edge: " + Edge);
            }
        }

        private static Size PerformLayoutBottom(Size canvasSize, int baselineHeight, int dockHeight, int iconSize, IEnumerable<DockItem> items)
        {
            var left = 0;
            var top = canvasSize.Height;

            foreach (var item in items)
            {
                item.Bounds = new Rectangle
                {
                    X = left + item.Margin.Left,
                    Y = item.WithinContainerBounds ? canvasSize.Height - dockHeight : canvasSize.Height - baselineHeight - iconSize + item.Margin.Top,
                    Height = item.WithinContainerBounds ? dockHeight : iconSize,
                    Width = item.WithinContainerBounds ? (int)(dockHeight * (item.Image.Width * 1F / item.Image.Height)) : iconSize
                };

                if (item.Y < top)
                    top = item.Y;

                left += item.Margin.Left + item.Width + item.Margin.Right;
            }

            var width = items.Sum(i => i.Margin.Left + i.Width + i.Margin.Right);
            left = (canvasSize.Width - width) / 2;

            foreach (var item in items)
            {
                item.X += left;
            }

            return new Size(width, canvasSize.Height - top);
        }

        private static Size PerformLayoutTop(Size canvasSize, int baselineHeight, int dockHeight, int iconSize, IEnumerable<DockItem> items)
        {
            var left = 0;
            var top = canvasSize.Height;

            foreach (var item in items)
            {
                item.Bounds = new Rectangle
                {
                    X = left + item.Margin.Left,
                    Y = item.WithinContainerBounds ? canvasSize.Height - dockHeight : canvasSize.Height - baselineHeight - iconSize + item.Margin.Top,
                    Height = item.WithinContainerBounds ? dockHeight : iconSize,
                    Width = item.WithinContainerBounds ? (int)(dockHeight * (item.Image.Width * 1F / item.Image.Height)) : iconSize
                };

                if (item.Y < top)
                    top = item.Y;

                left += item.Margin.Left + item.Width + item.Margin.Right;
            }

            var width = items.Sum(i => i.Margin.Left + i.Width + i.Margin.Right);
            left = (canvasSize.Width - width) / 2;

            foreach (var item in items)
            {
                item.X += left;
            }

            return new Size(width, canvasSize.Height - top);
        }

        private static Size PerformLayoutLeft(Size canvasSize, int baselineHeight, int dockHeight, int iconSize, IEnumerable<DockItem> items)
        {
            var left = 0;
            var top = canvasSize.Height;

            foreach (var item in items)
            {
                item.Bounds = new Rectangle
                {
                    X = left + item.Margin.Left,
                    Y = item.WithinContainerBounds ? canvasSize.Height - dockHeight : canvasSize.Height - baselineHeight - iconSize + item.Margin.Top,
                    Height = item.WithinContainerBounds ? dockHeight : iconSize,
                    Width = item.WithinContainerBounds ? (int)(dockHeight * (item.Image.Width * 1F / item.Image.Height)) : iconSize
                };

                if (item.Y < top)
                    top = item.Y;

                left += item.Margin.Left + item.Width + item.Margin.Right;
            }

            var width = items.Sum(i => i.Margin.Left + i.Width + i.Margin.Right);
            left = (canvasSize.Width - width) / 2;

            foreach (var item in items)
            {
                item.X += left;
            }

            return new Size(width, canvasSize.Height - top);
        }

        private static Size PerformLayoutRight(Size canvasSize, int baselineHeight, int dockHeight, int iconSize, IEnumerable<DockItem> items)
        {
            var left = 0;
            var top = canvasSize.Height;

            foreach (var item in items)
            {
                item.Bounds = new Rectangle
                {
                    X = left + item.Margin.Left,
                    Y = item.WithinContainerBounds ? canvasSize.Height - dockHeight : canvasSize.Height - baselineHeight - iconSize + item.Margin.Top,
                    Height = item.WithinContainerBounds ? dockHeight : iconSize,
                    Width = item.WithinContainerBounds ? (int)(dockHeight * (item.Image.Width * 1F / item.Image.Height)) : iconSize
                };

                if (item.Y < top)
                    top = item.Y;

                left += item.Margin.Left + item.Width + item.Margin.Right;
            }

            var width = items.Sum(i => i.Margin.Left + i.Width + i.Margin.Right);
            left = (canvasSize.Width - width) / 2;

            foreach (var item in items)
            {
                item.X += left;
            }

            return new Size(width, canvasSize.Height - top);
        }
    }
}