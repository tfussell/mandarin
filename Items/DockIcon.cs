using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace WinDock
{
    class DockIcon
    {
        public String DisplayName { get; set; }
        public Bitmap Bitmap { get; protected set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        private bool hover = false;

        public DockIcon(DockIcon d)
        {
            DisplayName = d.DisplayName;
            Bitmap = d.Bitmap;
            Width = d.Width;
            Height = d.Height;
            X = d.X;
            Y = d.Y;
        }

        public DockIcon()
        {
            DisplayName = null;
            Bitmap = null;
        }

        public virtual void OnHover()
        {
            hover = true;
        }

        public virtual void OnClick()
        {

        }

        public virtual void Update(int index)
        {

        }

        public virtual void PaintTooltip(Graphics graphics)
        {
            float font_size = 10;
            String font_name = "Verdana";
            Font tooltip_font = new Font(font_name, font_size, FontStyle.Bold);
            SizeF tooltip_text_size = graphics.MeasureString(DisplayName, tooltip_font);
            int left = X + (Configuration.IconSize / 2) - (int)(tooltip_text_size.Width / 2);

            // Base rectangle
            Rectangle tooltip_rectangle = new Rectangle(left, Y - 40, (int)tooltip_text_size.Width, (int)tooltip_text_size.Height + 4);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), tooltip_rectangle);

            // Tooltip text
            System.Drawing.Drawing2D.SmoothingMode old = graphics.SmoothingMode;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            graphics.DrawString(DisplayName, tooltip_font, new SolidBrush(Color.FromArgb(200, 255, 255, 255)), new Point(left, Y - 38));
            graphics.SmoothingMode = old;

            // Rounded corners
            graphics.FillPie(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), new Rectangle(left - 10, Y - 40, 20, (int)tooltip_text_size.Height + 4), 90, 180);
            graphics.FillPie(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), new Rectangle(left + (int)tooltip_text_size.Width - 10, Y - 40, 20, (int)tooltip_text_size.Height + 4), 270, 180);

            // Arrow triangle
            Point[] tri_points = { new Point(left + (int)(tooltip_text_size.Width / 2) - 5, Y - 40 + 4 + (int)tooltip_text_size.Height),
                                             new Point(left + (int)(tooltip_text_size.Width / 2) + 5, Y - 40 + 4 + (int)tooltip_text_size.Height),
                                             new Point(left + (int)(tooltip_text_size.Width / 2), Y - 40 + 4 + (int)tooltip_text_size.Height + 5) };
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), tri_points);
        }

        public virtual void Paint(Graphics graphics)
        {
            if (hover)
            {
                //PaintTooltip(graphics);
                hover = false;
            }

            graphics.DrawImage(Bitmap, X, Y, Width, Height);
        }
    }
}
