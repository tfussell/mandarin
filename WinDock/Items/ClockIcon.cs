using System;
using System.Windows.Forms;
using System.Drawing;
using WinDock.GUI;

namespace WinDock.Items
{
    class ClockIcon : DockItem
    {
        private readonly Timer timer;
        private string timeString = "";

        public ClockIcon()
        {
            timer = new Timer {Interval = 1000};
            timer.Tick += UpdateTime;
            timer.Start();

            Image = new Bitmap(1, 1);

            var currentTime = DateTime.Now;
            timeString = currentTime.ToShortTimeString();
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now;
            timeString = currentTime.ToShortTimeString();
        }

        public void PaintToBuffer(Graphics graphics)
        {
            float font_size = 9;
            String font_name = "Segoe UI";
            Font tooltip_font = new Font(font_name, font_size, FontStyle.Regular);
            SizeF tooltip_text_size = graphics.MeasureString(timeString, tooltip_font);
            int left = X + (int)(Width / 2) - (int)(tooltip_text_size.Width / 2);
            int top = Y + (int)(Height / 2) - (int)(tooltip_text_size.Height / 2);

            System.Drawing.Drawing2D.SmoothingMode old = graphics.SmoothingMode;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Base rectangle
            Rectangle tooltip_rectangle = new Rectangle(X, Y, Width, Height);
            graphics.FillEllipse(new SolidBrush(Color.FromArgb(150, 20, 20, 20)), tooltip_rectangle);
            graphics.DrawEllipse(new Pen(Color.FromArgb(200, 200, 200, 200), 1.6F), tooltip_rectangle);

            graphics.DrawString(timeString, tooltip_font, new SolidBrush(Color.FromArgb(255, 255, 255, 255)), new Point(left, top));
            graphics.SmoothingMode = old;
        }
    }
}
