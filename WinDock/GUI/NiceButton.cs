using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace WinDock.GUI
{
    internal class NiceButton : Button
    {
        private bool hover;
        private bool toggled;

        public NiceButton(string text)
        {
            Text = text;
            Width = 300;
            Height = 50;

            Margin = new Padding {Left = 0, Right = 0, Top = 5, Bottom = 5};
            Padding = Padding.Empty;

            MouseEnter += (s, e) => Hover = true;
            MouseLeave += (s, e) => Hover = false;
            MouseDown += (s, e) => Toggled = true;

            NormalBackColor = Color.FromArgb(34, 39, 42);
            BackColor = NormalBackColor;
            HoverBackColor = Color.FromArgb(30, 32, 34);
            ToggledBackColor = Color.FromArgb(27, 29, 31);
        }

        public Color NormalBackColor { get; set; }
        public Color ToggledBackColor { get; set; }
        public Color HoverBackColor { get; set; }

        public bool Hover
        {
            get { return hover; }
            set
            {
                hover = value;

                if (hover && !toggled)
                    BackColor = HoverBackColor;
                else if (toggled)
                    BackColor = ToggledBackColor;
                else
                    BackColor = NormalBackColor;

                Refresh();
            }
        }

        public bool Toggled
        {
            get { return toggled; }
            set
            {
                toggled = value;
                if (toggled)
                    BackColor = ToggledBackColor;
                else if (hover)
                    BackColor = HoverBackColor;
                else
                    BackColor = NormalBackColor;
                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(BackColor);
            pevent.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            if (toggled)
            {
                pevent.Graphics.DrawRectangle(new Pen(Color.Black, 2.5F), Bounds);
                pevent.Graphics.DrawLine(new Pen(Color.FromArgb(50, 53, 56), 1.5F), 0, Height - 1, Width, Height - 1);
                pevent.Graphics.DrawString(Text, new Font("Segoe UI", 11), new SolidBrush(Color.White), 10, 12);
            }
            else
            {
                pevent.Graphics.DrawString(Text, new Font("Segoe UI", 11), new SolidBrush(Color.Gray), 10, 12);
            }
        }
    }
}