using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace WinDock
{
    class DockWindow : TransparentWindow
    {
        private Bitmap bitmap;
        private Bitmap running_indicator;
        Timer timer = new Timer();

        public DockWindow()
        {
            String roaming_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String taskbar_shortcut_directory = roaming_path + "\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar";

            Icons = LoadIconsFromDirectory(taskbar_shortcut_directory);

            AllowDrop = true;
            DragDrop += new DragEventHandler(HandleDragDrop);
            DragEnter += new DragEventHandler(HandleDragEnter);
            DragLeave += new EventHandler(HandleDragLeave);
            DragOver += new DragEventHandler(HandleDragOver);

            MouseMove += new MouseEventHandler(HandleMouseMove);
            MouseClick += new MouseEventHandler(HandleMouseClick);

            timer.Tick += new EventHandler(HandleTimerTick);
            timer.Interval = 200;
            timer.Enabled = true;
            timer.Start();

            this.bitmap = new Bitmap(Configuration.CanvasWidth, Configuration.CanvasHeight, PixelFormat.Format32bppArgb);

            UpdateBitmap();
            SetBitmap(this.bitmap);

            running_indicator = (Bitmap)Image.FromFile("C:\\Users\\William\\Desktop\\indicator.png");

            placeholderIndex = -1;
            hoverIndex = -1;
        }

        void HandleTimerTick(object sender, EventArgs e)
        {
            if (PointToClient(Cursor.Position).Y < 100)
            {
                hoverIndex = -1;
            }
            else
            {
                hoverIndex = GetIndex(PointToClient(Cursor.Position).X);
            }

            this.Refresh();
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (placeholderIndex != -1)
            {
                placeholderIndex = GetIndex(e.X);
            }
            else
            {
                hoverIndex = GetIndex(e.X);
            }

            this.Refresh();
        }

        private int GetIndex(int x)
        {
            DockWidth = Icons.Count * (Configuration.IconSize + Configuration.IconMargin) + Configuration.IconMargin * 2;

            if (placeholderIndex != -1)
                DockWidth += Configuration.IconSize + 2 * Configuration.IconMargin;

            Point top_left = new Point((Configuration.CanvasWidth - DockWidth) / 2, Configuration.CanvasHeight - Configuration.DockHeight);

            if (x < top_left.X || x > top_left.X + DockWidth)
            {
                return -1;
            }

            int index = (x - top_left.X) / (Configuration.IconSize + Configuration.IconMargin);

            if (index < 0 || index > Icons.Count)
            {
                return -1;
            }

            return index;
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            int index = GetIndex(e.X);

            if (index >= 0)
            {
                DockIcon icon = Icons[index];
                icon.Launch();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            UpdateBitmap();
            SetBitmap(this.bitmap);
        }

        void DrawBackground(Graphics graphics)
        {
            Point top_left = new Point((Configuration.CanvasWidth - DockWidth) / 2, Configuration.CanvasHeight - Configuration.DockHeight);
            Point bottom_right = new Point(top_left.X + DockWidth, Configuration.CanvasHeight - 3);

            Point[] trap_points = {new Point(top_left.X, top_left.Y),
                                   new Point(top_left.X - Configuration.DockSideSlope, bottom_right.Y), 
                                   new Point(bottom_right.X + Configuration.DockSideSlope, bottom_right.Y), 
                                   new Point(bottom_right.X, top_left.Y)};

            Point[] line_points = {new Point(top_left.X - Configuration.DockSideSlope, bottom_right.Y), 
                                   new Point(bottom_right.X + Configuration.DockSideSlope, bottom_right.Y)};

            Point[] edge_points = {new Point(top_left.X - Configuration.DockSideSlope, bottom_right.Y + 2), 
                                   new Point(bottom_right.X + Configuration.DockSideSlope, bottom_right.Y + 2)};

            int alpha = (int)(Configuration.DockBackgroundAlpha * 255);
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(alpha, Configuration.DockBackgroundColor)), trap_points);
            graphics.DrawLine(new Pen(Color.FromArgb(alpha, 50, 50, 50), 0.8F), line_points[0], line_points[1]);
            graphics.DrawLine(new Pen(Color.FromArgb(alpha, 180, 180, 180), 3), edge_points[0], edge_points[1]);
        }

        void DrawIcons(Graphics graphics)
        {
            Point current = new Point((Configuration.CanvasWidth - DockWidth) / 2 + Configuration.IconMargin, 136);
            int count = 0;

            foreach (DockIcon icon in Icons)
            {
                if(count == placeholderIndex)
                {
                    current.X += Configuration.IconSize + Configuration.IconMargin;
                }
                else if (count == hoverIndex)
                {
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    float font_size = 10;
                    String font_name = "Verdana";
                    Font tooltip_font = new Font(font_name, font_size, FontStyle.Bold);
                    SizeF tooltip_text_size = graphics.MeasureString(icon.DisplayName, tooltip_font);
                    int left = current.X + (Configuration.IconSize / 2) - (int)(tooltip_text_size.Width / 2);
                    Rectangle tooltip_rectangle = new Rectangle(left, current.Y - 40, (int)tooltip_text_size.Width, (int)tooltip_text_size.Height + 4);
                    graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), tooltip_rectangle);
                    graphics.DrawString(icon.DisplayName, tooltip_font, new SolidBrush(Color.FromArgb(200, 255, 255, 255)), new Point(left, current.Y - 38));
                    graphics.FillPie(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), new Rectangle(left - 10, current.Y - 40, 20, (int)tooltip_text_size.Height + 4), 90, 180);
                    graphics.FillPie(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), new Rectangle(left + (int)tooltip_text_size.Width - 10, current.Y - 40, 20, (int)tooltip_text_size.Height + 4), 270, 180);
                    Point[] tri_points = { new Point(left + (int)(tooltip_text_size.Width / 2) - 5, current.Y - 40 + 4 + (int)tooltip_text_size.Height),
                                             new Point(left + (int)(tooltip_text_size.Width / 2) + 5, current.Y - 40 + 4 + (int)tooltip_text_size.Height),
                                             new Point(left + (int)(tooltip_text_size.Width / 2), current.Y - 40 + 4 + (int)tooltip_text_size.Height + 5) };
                    graphics.FillPolygon(new SolidBrush(Color.FromArgb(200, 50, 50, 50)), tri_points);
                }

                count++;

                graphics.DrawImage(icon.Bitmap, current.X, current.Y, Configuration.IconSize, Configuration.IconSize);
                graphics.DrawImage(icon.ReflectionBitmap, current.X, current.Y + Configuration.IconSize, Configuration.IconSize, Configuration.IconSize);

                if (icon.Running)
                {
                    graphics.DrawImage(running_indicator, current.X + 17, current.Y + 51, 10, 10);
                }

                current.X += Configuration.IconSize + Configuration.IconMargin;
            }
        }

        private void UpdateBitmap()
        {
            DockWidth = Icons.Count * (Configuration.IconSize + Configuration.IconMargin) + Configuration.IconMargin * 2;

            if (placeholderIndex != -1)
            {
                DockWidth += Configuration.IconSize + 2 * Configuration.IconMargin;
            }

            using (Graphics graphics = Graphics.FromImage(this.bitmap))
            {
                graphics.Clear(BlankColor);

                DrawBackground(graphics);
                //DrawScreenReflection(graphics);
                DrawIcons(graphics);
            }
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    e.Effect = DragDropEffects.Copy;
                    Icons.Insert(GetIndex(PointToClient(new Point(e.X, e.Y)).X), new DockIcon(files[0]));
                }
            }

            placeholderIndex = -1;
        }

        private void HandleDragOver(object sender, DragEventArgs e)
        {
            if (placeholderIndex != -1)
                placeholderIndex = GetIndex(PointToClient(new Point(e.X, e.Y)).X);

            this.Refresh();
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    e.Effect = DragDropEffects.Copy;
                    placeholderIndex = GetIndex(PointToClient(new Point(e.X, e.Y)).X);
                }
            }
        }

        private void HandleDragLeave(object sender, EventArgs e)
        {
            placeholderIndex = -1;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && Icons != null)
                {
                    foreach (DockIcon icon in Icons)
                    {
                        icon.Bitmap.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private List<DockIcon> LoadIconsFromDirectory(String directory)
        {
            String[] files = Directory.GetFiles(directory);
            List<DockIcon> icons = new List<DockIcon>(files.Length);

            foreach (String file in files)
            {
                if (!file.EndsWith("desktop.ini"))
                {
                    icons.Add(new DockIcon(file));
                }
            }

            return icons;
        }

        public List<DockIcon> Icons { get; set; }
        private FloatingDockIcon FloatingIcon { get; set; }
        private static Color BlankColor = Color.FromArgb(0, 0, 0, 0);
        private int DockWidth { get; set; }
        private int placeholderIndex;
        private int hoverIndex;
    }
}
