using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace WinDock.GUI
{
    internal class NiceLabel : Label
    {
        public NiceLabel(string text)
        {
            Text = text;
            Width = 100;
            Height = 30;

            Margin = new Padding {Left = 0, Right = 0, Top = 5, Bottom = 5};
            Padding = Padding.Empty;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(BackColor);
            pevent.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            pevent.Graphics.DrawString(Text, new Font("Segoe UI", 11), new SolidBrush(Color.Black), 0, 0);
        }
    }
}