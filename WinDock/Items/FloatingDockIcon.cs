using System.Drawing;
using WinDock.GUI;

namespace WinDock.Items
{
    class FloatingDockIcon : TransparentWindow
    {
        public DockItem Subject
        {
            get { return subject; }
            set
            {
                if (Equals(subject, value)) return;
                subject = value;

                if (subject != null && subject.Image != null)
                {
                    Size = subject.Image.Size;
                    Visible = true;
                    Redraw();
                }
                else
                {
                    Visible = false;
                }
            }
        }

        public Point MouseOffset { get; set; }

        private DockItem subject;

        public FloatingDockIcon()
        {

        }

        protected override void RenderToBuffer(Graphics buffer)
        {
            buffer.Clear(Color.Transparent);
            buffer.DrawImage(subject.Image, 0, 0, Width, Height);
        }
    }
}
