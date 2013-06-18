using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinDock.Items;

namespace WinDock.Drawing
{
    internal class ForegroundPainter
    {
        private readonly Color clearColor;

        public ForegroundPainter()
        {
            clearColor = Color.FromArgb(0, 0, 0, 0);
        }

        public void Paint(List<DockItem> tiles, Graphics canvas, IEnumerable<Rectangle> dirtyRectangles = null)
        {
            if (dirtyRectangles == null)
            {
                canvas.Clear(clearColor);
                foreach (DockItem tile in tiles)
                {
                    PaintTile(tile, canvas);
                }
            }
            else
            {
                foreach (var dirtyRegion in dirtyRectangles)
                {
                    canvas.FillRectangle(new SolidBrush(clearColor), dirtyRegion);
                    Rectangle region = dirtyRegion;
                    foreach (var tile in tiles.Where(tile => tile.Bounds.IntersectsWith(region)))
                    {
                        PaintTile(tile, canvas);
                    }
                }
            }
        }

        private void PaintTile(DockItem tile, Graphics canvas)
        {
        }
    }
}