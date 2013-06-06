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
        // Icons
        private List<DockIcon> icons = null;
        private FloatingDockIcon floatingIcon = null;

        // Drawing
        private Bitmap bitmap = null;
        private Bitmap runningIndicator = null;
        private Timer refreshTimer = null;
        private static Color blankColor = Color.FromArgb(0, 0, 0, 0);

        // Other
        private int numAppIcons = 0;
        private int dockWidth = 0;
        private int placeholderIndex = -1;
        private int hoverIndex = -1;
        private int mouseDownX = -1;
        private int mouseDownY = -1;

        public DockWindow()
        {
            InitDragDropEvents();
            InitMouseEvents();
            InitIcons();
            InitRefreshTimer();
            InitBitmap();

            floatingIcon = new FloatingDockIcon();
            Bitmap b = new Bitmap(Configuration.IconSize, Configuration.IconSize);
            using (Graphics g = Graphics.FromImage(b))
            {
                Bitmap i = (Bitmap)Image.FromFile(Configuration.IconsFolder + Path.DirectorySeparatorChar + Configuration.RecycleBinImageFilenameE);
                g.DrawImage(i, new Rectangle(0, 0, Configuration.IconSize, Configuration.IconSize));
            }
            floatingIcon.SetBitmap(b);
            floatingIcon.MouseOffset = new Point(Configuration.IconSize / 2, Configuration.IconSize / 2);
        }

        private void InitIcons()
        {
            icons = LoadAppIcons();
            numAppIcons = icons.Count;

            icons.Add(new SeparatorIcon());
            icons.AddRange(LoadFolderIcons());
            icons.Add(new RecycleBinIcon());
        }

        private List<DockIcon> LoadFolderIcons()
        {
            return new List<DockIcon>();
        }

        private void InitBitmap()
        {
            this.bitmap = new Bitmap(Configuration.CanvasWidth, Configuration.CanvasHeight, PixelFormat.Format32bppArgb);

            UpdateBitmap();
            SetBitmap(this.bitmap);
        }

        private void InitRefreshTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Tick += new EventHandler(HandleTimerTick);
            refreshTimer.Interval = 100;
            refreshTimer.Enabled = true;
            refreshTimer.Start();
        }

        private void InitMouseEvents()
        {
            MouseMove += new MouseEventHandler(HandleMouseMove);
            MouseClick += new MouseEventHandler(HandleMouseClick);
            MouseDown += new MouseEventHandler(HandleMouseDown);
            MouseUp += new MouseEventHandler(HandleMouseUp);
        }

        private void InitDragDropEvents()
        {
            AllowDrop = true;

            DragDrop += new DragEventHandler(HandleDragDrop);
            DragEnter += new DragEventHandler(HandleDragEnter);
            DragLeave += new EventHandler(HandleDragLeave);
            DragOver += new DragEventHandler(HandleDragOver);
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
                if (hoverIndex >= 0 && hoverIndex < icons.Count)
                {
                    icons[hoverIndex].OnHover();
                }
            }

            if (mouseDownX != -1 && mouseDownY != -1)
            {
                if (e.X != mouseDownX && e.Y != mouseDownY && !floatingIcon.Visible)
                {
                    floatingIcon.Show();
                }

                floatingIcon.SetDesktopLocation(Cursor.Position.X - floatingIcon.MouseOffset.X, Cursor.Position.Y - floatingIcon.MouseOffset.Y);
            }

            this.Refresh();
        }

        private int CalculateDockWidth()
        {
            int width = 0;

            foreach (DockIcon icon in icons)
            {
                width += icon.Width;
                width += Configuration.IconMargin;
            }

            if (placeholderIndex != -1)
            {
                width += Configuration.IconSize + Configuration.IconMargin;
            }

            return width;
        }

        private int GetIndex(int x)
        {
            dockWidth = CalculateDockWidth();
            Point top_left = new Point((Configuration.CanvasWidth - dockWidth) / 2, Configuration.CanvasHeight - Configuration.DockHeight);

            if (x < top_left.X || x > top_left.X + dockWidth)
            {
                return -1;
            }

            int index = (x - top_left.X) / (Configuration.IconSize + Configuration.IconMargin);

            if (index < 0 || index > icons.Count)
            {
                return -1;
            }

            return index;
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            int index = GetIndex(e.X);

            if (index >= 0 && index < icons.Count)
            {
                DockIcon icon = icons[index];
                icon.OnClick();
            }
        }

        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            mouseDownX = -1;
            mouseDownY = -1;
            floatingIcon.Hide();
            this.Refresh();
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            mouseDownX = e.X;
            mouseDownY = e.Y;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            UpdateBitmap();
            SetBitmap(this.bitmap);
        }

        void DrawBackground(Graphics graphics)
        {
            Point top_left = new Point((Configuration.CanvasWidth - dockWidth) / 2, Configuration.CanvasHeight - Configuration.DockHeight);
            Point bottom_right = new Point(top_left.X + dockWidth, Configuration.CanvasHeight - 3);

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

        void UpdateIcons()
        {
            Point current = new Point((Configuration.CanvasWidth - dockWidth) / 2 + Configuration.IconMargin, 136);
            int count = 0;

            foreach (DockIcon icon in icons)
            {
                if (count == placeholderIndex)
                {
                    current.X += Configuration.IconSize + Configuration.IconMargin;
                }
                count++;

                icon.X = current.X;
                icon.Y = current.Y;

                if (count == numAppIcons + 1)
                {
                    icon.Y += 20;
                }

                icon.Update(count);

                current.X += icon.Width + Configuration.IconMargin;
            }
        }

        void DrawIcons(Graphics graphics)
        {
            if (runningIndicator == null)
            {
                runningIndicator = (Bitmap)Image.FromFile("C:\\Users\\William\\Desktop\\iconsFolder\\indicator.png");
            }

            int count = 0;

            foreach (DockIcon icon in icons)
            {
                count++;
                icon.Paint(graphics);
            }
        }

        private void UpdateBitmap()
        {
            dockWidth = CalculateDockWidth();
            UpdateIcons();

            using (Graphics graphics = Graphics.FromImage(this.bitmap))
            {
                graphics.Clear(blankColor);

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
                    icons.Insert(GetIndex(PointToClient(new Point(e.X, e.Y)).X), new ApplicationIcon(files[0]));
                    numAppIcons++;
                }
            }

            placeholderIndex = -1;
        }

        private void HandleDragOver(object sender, DragEventArgs e)
        {
            if (placeholderIndex != -1)
            {
                placeholderIndex = GetIndex(PointToClient(new Point(e.X, e.Y)).X);
                if (placeholderIndex > numAppIcons)
                {
                    placeholderIndex = -1;
                }
            }


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
                if (disposing && icons != null)
                {
                    foreach (DockIcon icon in icons)
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

        private List<DockIcon> LoadAppIcons()
        {
            String directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            directory += Configuration.TaskbarPinnedDirectory;

            String[] files = Directory.GetFiles(directory);
            List<DockIcon> icons = new List<DockIcon>(files.Length);

            foreach (String file in files)
            {
                if (!file.EndsWith("desktop.ini"))
                {
                    icons.Add(new ApplicationIcon(file));
                }
            }

            return icons;
        }
    }
}
