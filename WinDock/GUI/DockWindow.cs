using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using HookWrapper;
using WinDock.Dock;
using WinDock.Items;

namespace WinDock.GUI
{
    public class DockWindow : TransparentWindow
    {
        public delegate void FileDropDelegate(object sender, DockDropEventArgs e);
        public event FileDropDelegate FileDrop = delegate { }; 

        public ScreenEdge Edge
        {
            get { return edge; }
            set
            {
                if (value.Equals(edge)) return;
                edge = value;
                OnEdgeChange(edge);
            }
        }

        private void OnEdgeChange(ScreenEdge newEdge)
        {
            layoutManager.Edge = newEdge;
            layoutManager.PerformLayout(Size, baselineHeight, backgroundHeight, iconSize, items.AllItems);
            Redraw();
        }

        public int ScreenIndex
        {
            set
            {
                if (screenIndex == value) return;
                screenIndex = value;
                OnScreenIndexChange(screenIndex);
            }
        }

        public bool Autohide
        {
            set
            {
                if (autohide == value) return;
                if (autohide) DisableAutohide();
                else EnableAutohide();
            }
        }

        public bool Reserve
        {
            set
            {
                if (reserve == value) return;
                if (reserve) DisableReserveEdge();
                else EnableReserveEdge();
            }
        }

        public Screen Screen
        {
            get { return screen; }
            private set { screen = value; }
        }

        private const int EmptyWidth = 50;
        private int backgroundHeight;
        private int baselineHeight;
        private int iconSize;
        private Rectangle itemContainerBounds;
        private ItemGroupList items;
        private LayoutManager layoutManager;
        private int screenIndex;
        private ScreenEdge edge;
        private bool autohide;
        private bool reserve;
        private Screen screen;
        private RightClickMenu rightClickMenu;
        private Themes.DockStyle theme;
        private DockItemTooltipWindow tooltipWindow;

        public DockWindow()
        {
            Initialize();
        }

        private void EnableAutohide()
        {
            autohide = true;
        }

        private void DisableAutohide()
        {
            autohide = false;
        }

        private void EnableReserveEdge()
        {
            reserve = true;
        }

        private void DisableReserveEdge()
        {
            reserve = false;
        }

        private void OnScreenIndexChange(int screenEdge)
        {
            if (screenEdge < 0 || screenEdge >= Screen.AllScreens.Count()) return;
            Screen = Screen.AllScreens[screenEdge];
            Redraw();
        }

        void RegisterItem(DockItem item)
        {
            item.Style = theme.DockItemStyle;
            item.NameChanged += ItemNameChanged;
            item.PaintRequested += ItemPaintRequested;
        }

        void UnregisterItem(DockItem item)
        {
            item.NameChanged -= ItemNameChanged;
            item.PaintRequested -= ItemPaintRequested;
        }

        void ItemNameChanged(object sender, EventArgs e)
        {
            if (tooltipWindow.Visible)
            {
                tooltipWindow.Text = ((DockItem) sender).Name;
            }
        }

        void ItemPaintRequested(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Initialize()
        {
            itemContainerBounds = new Rectangle(0, 0, 0, 0);
            /*
            baselineHeight = 25;
            iconSize = 66;
            backgroundHeight = 50;
             */
            baselineHeight = 20;
            iconSize = 45;
            backgroundHeight = 35;
            screenIndex = 0;
            screen = Screen.AllScreens[screenIndex];
            edge = ScreenEdge.Bottom;
            rightClickMenu = new RightClickMenu(this);
            AddOwnedForm(rightClickMenu);

            items = new ItemGroupList();
            AddItemGroup(new ApplicationIconGroup());
            AddItemGroup(new SingleDockItemGroup(new SeparatorItem()));
            AddItemGroup(new FolderIconGroup());
            AddItemGroup(new SingleDockItemGroup(new RecycleBinIcon()));

            layoutManager = new LayoutManager(edge);

            Load += (sender, args) => CalculateBounds();
            Resize += (sender, args) => CalculateBounds();

            AllowDrop = true;

            //Load += (sender, args) => HookWrapperManaged.Instance.Initialize();
            //Closing += (sender, args) => HookWrapperManaged.Instance.Shutdown();
        }

        private void AddItemGroup(DockItemGroup group)
        {
            items.AddGroup(group);
            group.ItemsChanged += ItemGroupItemsChanged;
        }

        private void ItemGroupItemsChanged(object sender, ItemsChangedArgs args)
        {
            args.AddedItems.ToList().ForEach(RegisterItem);
            args.RemovedItems.ToList().ForEach(UnregisterItem);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = e.AllowedEffect & DragDropEffects.Copy;
                DropTargetHelper.DragEnter(this, e.Data, new Point(e.X, e.Y), e.Effect, "Pin to %1", "Dock");
            }
            else
            {
                e.Effect = DragDropEffects.None;
                DropTargetHelper.DragEnter(this, e.Data, new Point(e.X, e.Y), e.Effect);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect & DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
            DropTargetHelper.DragOver(new Point(e.X, e.Y), e.Effect);
        }

        protected override void  OnDragLeave(EventArgs e)
        {
            DropTargetHelper.DragLeave(this);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect & DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
            DropTargetHelper.Drop(e.Data, new Point(e.X, e.Y), e.Effect);

            if (e.Effect == DragDropEffects.Copy)
                AcceptFileDrop(e.Data);
        }

        private void AcceptFileDrop(IDataObject data)
        {
            if (!data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;

            foreach (var file in files)
            {
                var e = new DockDropEventArgs
                    {
                        File = file,
                        Index = CalculateInsertIndex(Cursor.Position.X, Cursor.Position.Y)
                    };
                FileDrop(this, e);
            }

            CalculateBounds();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            var pos = Cursor.Position;
            if (!rightClickMenu.Visible) return;
            if(!rightClickMenu.Bounds.IntersectsWith(new Rectangle(pos.X - 30, pos.Y - 30, 60, 60)))
                rightClickMenu.Hide();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool hoveringOverItem = items.AllItems.Any(i => i.Bounds.Contains(e.X, e.Y));
            if(!hoveringOverItem && rightClickMenu.Visible)
                rightClickMenu.Hide();
        }

        private int CalculateInsertIndex(int x, int y)
        {
            return (x + y) / (x + y);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            bool handled = items.AllItems.Any(i => i.HandleMouseClickEvent(this, e));
        }

        private void CalculateBounds()
        {
            if (CalculateItemContainerBounds() | CalculateWindowBounds()) Redraw();
        }

        private bool CalculateItemContainerBounds()
        {
            Rectangle oldBounds = itemContainerBounds;
            Size itemContainerSize = layoutManager.PerformLayout(Size, baselineHeight, backgroundHeight, iconSize, items.AllItems);
            if (itemContainerSize.Width < EmptyWidth) itemContainerSize.Width = EmptyWidth;
            Point itemContainerPoint = CalculateCeneteredLocation(itemContainerSize);
            itemContainerBounds = new Rectangle(itemContainerPoint, itemContainerSize);
            return !oldBounds.Equals(itemContainerBounds);
        }

        private Point CalculateCeneteredLocation(Size size)
        {
            return new Point((Width - size.Width)/2, Height - size.Height);
        }

        private bool CalculateWindowBounds()
        {
            Rectangle oldBounds = Bounds;
            Bounds = new Rectangle(screen.WorkingArea.Left, screen.WorkingArea.Bottom - 200, screen.WorkingArea.Width,
                                   200);
            TopMost = true;
            if (oldBounds.Equals(Bounds)) return false;
            return true;
        }

        protected override void RenderToBuffer(Graphics buffer)
        {
            buffer.Clear(Color.FromArgb(0, 0, 0, 0));

            buffer.InterpolationMode = InterpolationMode.HighQualityBicubic;
            buffer.CompositingQuality = CompositingQuality.HighQuality;
            buffer.SmoothingMode = SmoothingMode.AntiAlias;

            Brush polygonBrush = new SolidBrush(Color.FromArgb(230, 120, 120, 120));
            Point[] points =
                {
                    new Point(itemContainerBounds.Left, Height - backgroundHeight),
                    new Point(itemContainerBounds.Left - 30, Height - 3),
                    new Point(itemContainerBounds.Right + 30, Height - 3),
                    new Point(itemContainerBounds.Right, Height - backgroundHeight)
                };
            buffer.FillPolygon(polygonBrush, points);
            var darkLinePen = new Pen(Color.FromArgb(230, 90, 90, 90), 1F);
            var lightLinePen = new Pen(Color.FromArgb(230, 200, 200, 200), 3F);
            buffer.DrawLine(darkLinePen,
                       new Point(itemContainerBounds.Left - 30, Height - 3),
                       new Point(itemContainerBounds.Right + 30, Height - 3));
            buffer.DrawLine(lightLinePen,
                       new Point(itemContainerBounds.Left - 30, Height - 1),
                       new Point(itemContainerBounds.Right + 30, Height - 1));

            foreach (DockItem item in items.AllItems)
            {
                if (item.Image != null)
                    buffer.DrawImage(item.Image, item.Bounds);
                if (item.ReflectionImage != null)
                    buffer.DrawImage(item.ReflectionImage, new Rectangle(item.Bounds.X, item.Bounds.Y + item.Height - 6, item.Width, item.Height));
                if (item.GetType() == typeof (ApplicationDockItem))
                {
                    var appIcon = (ApplicationDockItem) item;
                    if (appIcon.Running)
                    {
                        buffer.DrawImage(appIcon.Indicator, new Rectangle(item.Bounds.X + item.Bounds.Width / 2 - 5, item.Bounds.Y + item.Height + 4, 10, 10));
                    }
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }
}