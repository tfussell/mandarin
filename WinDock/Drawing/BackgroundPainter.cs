using System.Drawing;
using WinDock.Dock;
using WinDock.GUI;

namespace WinDock.Drawing
{
    internal class BackgroundPainter
    {
        private readonly int cornerRadius;

        public BackgroundPainter(Color backColor, Color borderColor)
        {
            BackColor = backColor;
            BorderColor = borderColor;

            cornerRadius = 8;
        }

        private Color BackColor { get; set; }
        private Color BorderColor { get; set; }

        public void Paint(Graphics canvas, Rectangle bounds, ScreenEdge edge)
        {
            var f = EdgeFilterFromEdge(edge);
            canvas.FillRoundedRectangle(new SolidBrush(BackColor), bounds, cornerRadius, f);
            canvas.DrawRoundedRectangle(new Pen(BorderColor, 3.0F), bounds, cornerRadius, f);
        }

        private RectangleEdgeFilter EdgeFilterFromEdge(ScreenEdge edge)
        {
            var f = RectangleEdgeFilter.All;
            switch (edge)
            {
                case ScreenEdge.Bottom:
                    f |= RectangleEdgeFilter.BottomLeft;
                    f |= RectangleEdgeFilter.BottomRight;
                    break;
                case ScreenEdge.Left:
                    f |= RectangleEdgeFilter.BottomLeft;
                    f |= RectangleEdgeFilter.TopLeft;
                    break;
                case ScreenEdge.Top:
                    f |= RectangleEdgeFilter.TopLeft;
                    f |= RectangleEdgeFilter.TopRight;
                    break;
                case ScreenEdge.Right:
                    f |= RectangleEdgeFilter.TopRight;
                    f |= RectangleEdgeFilter.BottomRight;
                    break;
            }
            return f;
        }
    }
}