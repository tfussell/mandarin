using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using WinDock.Items;

namespace WinDock.GUI
{
    public abstract class RightClickMenuItem
    {
        protected Padding Padding;
        protected int height;
        protected int width;

        protected RightClickMenuItem()
        {
            Padding = new Padding(5, 3, 0, 3);
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public event Action Click = delegate { };

        protected virtual void OnClick()
        {
            Click();
        }

        public abstract void Paint(Graphics canvas, Point position);

        internal void HandleMouseDown(MouseEventArgs mouseEventArgs)
        {
            OnClick();
        }
    }

    internal class SeparatorRightClickMenuItem : RightClickMenuItem
    {
        protected Color LineColor;
        protected float LineWidth;

        public SeparatorRightClickMenuItem()
        {
            LineColor = Color.FromArgb(200, 200, 200, 200);
            LineWidth = 2F;

            height = 20;
            width = 200;
        }

        public override void Paint(Graphics canvas, Point position)
        {
            canvas.DrawLine(new Pen(LineColor, LineWidth),
                            new Point(position.X + 5, position.Y + Padding.Top + Height/2 - (int) (LineWidth/2)),
                            new Point(position.X + Width - 5, position.Y + Padding.Top + Height/2 - (int) (LineWidth/2)));
        }
    }

    internal class TextRightClickMenuItem : RightClickMenuItem
    {
        protected Font Font;
        protected string Text;

        public TextRightClickMenuItem(string text)
        {
            Text = text;
            Font = new Font("Lucida Sans Unicode", 11F, FontStyle.Regular);
        }

        public override void Paint(Graphics canvas, Point position)
        {
            var tooltipTextSize = canvas.MeasureString(Text, Font);
            const int left = 30;
            canvas.DrawString(Text, Font, new SolidBrush(Color.FromArgb(200, 255, 255, 255)), new Point(left, position.Y));
        }
    }

    internal class ToggleRightClickMenuItem : TextRightClickMenuItem
    {
        protected Image DisabledIcon;
        protected bool Enabled;
        protected Image EnabledIcon;

        public ToggleRightClickMenuItem(string disabledText, string enabledText, bool defaultState)
            : base(disabledText)
        {
            Enabled = defaultState;

            Click += () =>
                {
                    Enabled = !Enabled;
                    if (Enabled)
                    {
                        Text = enabledText;
                        ToggleOn();
                    }
                    else
                    {
                        Text = disabledText;
                        ToggleOff();
                    }
                };
        }

        public event Action ToggleOn = delegate { };
        public event Action ToggleOff = delegate { };

        public override void Paint(Graphics canvas, Point position)
        {
            base.Paint(canvas, position);

            if (!Enabled) return;
            const int left = 6;
            canvas.DrawString("✓", Font, new SolidBrush(Color.FromArgb(200, 150, 150, 150)),
                              new Point(left, position.Y));
        }
    }

    public class RightClickMenu : TransparentWindow
    {
        private readonly DockWindow parentDock;
        private List<RightClickMenuItem> contents;
        private DockItem subject;

        public RightClickMenu(DockWindow parent)
        {
            parentDock = parent;
            Initialize();
        }

        public void Initialize()
        {
            contents = new List<RightClickMenuItem>();
            MinimumSize = new Size(200, 25);
        }

        public void Show(DockItem newSubject)
        {
            subject = newSubject;
            UpdateContents();
            Size = CalculateSize();
            Location = CalculateLocation();
            Show();
        }

        private Size CalculateSize()
        {
            return new Size(200, 20 * contents.Count + 20);
        }

        private Point CalculateLocation()
        {
            Point itemOnScreen = parentDock.PointToScreen(subject.Bounds.Location);
            return new Point(itemOnScreen.X, itemOnScreen.Y - Height - 20);
        }

        private void UpdateContents()
        {
            contents.Clear();
            //parentDock.AddRightClickMenuItems(this);
            //contents.Add(new SeparatorRightClickMenuItem());
            subject.AddRightClickMenuItems(this);
        }

        public void AddToggleItem(string disabledText, string enabledText, Action enabled, Action disabled,
                                  bool defaultState = false)
        {
            var item = new ToggleRightClickMenuItem(disabledText, enabledText, defaultState);

            item.ToggleOff += disabled;
            item.ToggleOn += enabled;

            contents.Add(item);
        }

        protected override void RenderToBuffer(Graphics buffer)
        {
            buffer.TextRenderingHint = TextRenderingHint.AntiAlias;
            buffer.CompositingQuality = CompositingQuality.HighQuality;
            buffer.Clear(Color.FromArgb(0,0,0,0));
            var brush = new SolidBrush(Color.FromArgb(240, 50, 50, 50));
            const int radius = 5;
            buffer.FillRoundedRectangle(brush, new Rectangle(2, 2, Width - 4, Height - 4), radius);
            buffer.DrawRoundedRectangle(new Pen(Color.FromArgb(240, 255, 255, 255), 2F), new Rectangle(2, 2, Width - 4, Height - 4), radius);
            var position = new Point(0, 10);
            foreach (RightClickMenuItem item in contents)
            {
                item.Paint(buffer, position);
                position.Y += 20;
            }
        }

        public void AddTextItem(string text, Action action)
        {
            var item = new TextRightClickMenuItem(text);
            item.Click += action;
            contents.Add(item);
        }

        public void AddSeparatorItem()
        {
            contents.Add(new SeparatorRightClickMenuItem());
        }

        public void AddRange(List<RightClickMenuItem> items)
        {
            contents.AddRange(items);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Hide();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var position = new Point(0, 10);
            foreach (RightClickMenuItem item in contents)
            {
                if (e.Y > position.Y && e.Y < position.Y + 20)
                {
                    item.HandleMouseDown(e);
                    Hide();
                    return;
                }
                position.Y += 20;
            }
        }
    }
}