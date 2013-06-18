
using System.Drawing;

namespace WinDock.GUI
{
    public class DockItemTooltipWindow : TransparentWindow
    {
        protected override void RenderToBuffer(Graphics buffer)
        {
            const float fontSize = 10;
            const string fontName = "Verdana";
            var tooltipFont = new Font(fontName, fontSize, FontStyle.Bold);
            var tooltipTextSize = buffer.MeasureString(Text, tooltipFont);
            const int left = 5;

            // Base rectangle
            var tooltipRectangle = new Rectangle(left, 0, (int)tooltipTextSize.Width, (int)tooltipTextSize.Height + 4);
            buffer.FillRectangle(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), tooltipRectangle);

            // Tooltip text
            var old = buffer.SmoothingMode;
            buffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            buffer.DrawString(Text, tooltipFont, new SolidBrush(Color.FromArgb(200, 255, 255, 255)), new Point(left, 2));
            buffer.SmoothingMode = old;

            // Rounded corners
            buffer.FillPie(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), new Rectangle(left - 10, 0, 20, (int)tooltipTextSize.Height + 4), 90, 180);
            buffer.FillPie(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), new Rectangle(left + (int)tooltipTextSize.Width - 10, 0, 20, (int)tooltipTextSize.Height + 4), 270, 180);

            // Arrow triangle
            Point[] triPoints = { new Point(left + (int)(tooltipTextSize.Width / 2) - 5, 0 + 4 + (int)tooltipTextSize.Height),
                                             new Point(left + (int)(tooltipTextSize.Width / 2) + 5, 0 + 4 + (int)tooltipTextSize.Height),
                                             new Point(left + (int)(tooltipTextSize.Width / 2), 0 + 4 + (int)tooltipTextSize.Height + 5) };
            buffer.FillPolygon(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), triPoints);
        }
    }
}
