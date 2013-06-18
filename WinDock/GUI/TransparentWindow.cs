using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsShellFacade;

namespace WinDock.GUI
{
    public abstract class TransparentWindow : Form
    {
        private Bitmap windowBuffer;

        protected TransparentWindow()
        {
            Initialize();
        }

        protected void Redraw()
        {
            UpdateLayeredWindow();
        }

        private void Initialize()
        {
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            Load += (sender, args) => Redraw();
        }

        sealed protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        sealed protected override void OnPaint(PaintEventArgs e)
        {
            UpdateLayeredWindow();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
                //cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected abstract void RenderToBuffer(Graphics buffer);

        private void UpdateLayeredWindow()
        {
            if (windowBuffer == null || windowBuffer.Size != Size)
            {
                if (windowBuffer != null)
                {
                    windowBuffer.Dispose();
                }
                windowBuffer = new Bitmap(Width, Height);
            }

            using (var buffer = Graphics.FromImage(windowBuffer))
            {
                RenderToBuffer(buffer);
            }

            var screenDC = User32.GetDC(IntPtr.Zero);
            var memDC = Gdi32.CreateCompatibleDC(screenDC);
            var hBitmap = windowBuffer.GetHbitmap(Color.FromArgb(0, 0, 0, 0));
            var hPreviousBitmap = Gdi32.SelectObject(memDC, hBitmap);

            var size = windowBuffer.Size;
            var topPos = new Point(Left, Top);

            User32.UpdateLayeredWindow(Handle, screenDC, topPos, size, memDC);

            User32.ReleaseDC(IntPtr.Zero, screenDC);
            if (hBitmap != IntPtr.Zero)
            {
                Gdi32.SelectObject(memDC, hPreviousBitmap);
                Gdi32.DeleteObject(hBitmap);
            }
            Gdi32.DeleteDC(memDC);
        }
    }
}