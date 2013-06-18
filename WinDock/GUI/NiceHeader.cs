using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace WinDock.GUI
{
    internal class NiceHeader : Label
    {
        public NiceHeader(string text)
        {
            Text = text;
            Width = 300;
            Height = 50;
            Margin = new Padding {Left = 0, Right = 0, Top = 5, Bottom = 5};
            Padding = Padding.Empty;
            BackColor = Color.FromArgb(27, 29, 31);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.DrawString(Text.ToUpper(), new Font("Segoe UI", 9), new SolidBrush(Color.Gray), 10, 15);
        }
    }
}