using System.Drawing;
using System.Windows.Forms;

namespace WinDock.GUI
{
    internal sealed class ButtonList : UserControl
    {
        public delegate void SelectionChangedDelegate(string name);

        private readonly FlowLayoutPanel panel;
        private NiceButton current;

        public ButtonList(string text)
        {
            Text = text;

            Width = 300;
            Height = 500;

            AutoSize = true;

            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(34, 39, 42);

            panel = new FlowLayoutPanel {FlowDirection = FlowDirection.TopDown};
            panel.ControlAdded += (s, e) => Refresh();
            panel.ControlRemoved += (s, e) => Refresh();
            panel.AutoSize = true;
            panel.Padding = Padding.Empty;
            panel.Margin = Padding.Empty;
            Controls.Add(panel);

            Margin = Padding.Empty;
            Padding = Padding.Empty;
        }

        public string Selected
        {
            get { return current.Text; }
            set
            {
                foreach (NiceButton b in panel.Controls)
                {
                    if (b.Text == value)
                    {
                        current.Toggled = false;
                        current = b;
                        current.Toggled = true;
                        SelectionChanged(value);
                    }
                }
            }
        }

        public event SelectionChangedDelegate SelectionChanged = delegate { };

        public void AddButton(string text)
        {
            var b = new NiceButton(text);

            b.Click += (s, e) =>
                {
                    current.Toggled = false;
                    current = b;
                    current.Toggled = true;
                    SelectionChanged(text);
                };

            panel.Controls.Add(b);

            if (current == null)
            {
                current = b;
                current.Toggled = true;
            }
        }
    }
}