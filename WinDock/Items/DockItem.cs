using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using WinDock.GUI;
using WinDock.Themes;

namespace WinDock.Items
{
    /// <summary>
    /// An abstract class representing any bounded displayable item on a dock.
    /// </summary>
    public abstract class DockItem : IDisposable
    {
        #region Events
        public event EventHandler PaintRequested = delegate {  };
        public event EventHandler MouseHover = delegate { };
        public event MouseEventHandler MouseClick = delegate { };
        public event MouseEventHandler MouseDoubleClick = delegate { };
        public event EventHandler MouseEnter = delegate { };
        public event EventHandler MouseLeave = delegate { };
        public event EventHandler NameChanged = delegate { };
        #endregion

        #region Properties
        private bool Active
        {
            get { return active; }
            set
            {
                if (value == active) return;
                active = value;
                OnRequestPaint();
            }
        }

        public Image Image
        {
            get { return image; }
            protected set
            {
                if (Equals(image, value)) return;
                image = value;
                OnImageChange(image);
            }
        }

        public Image ReflectionImage { get; protected set; }

        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public int X
        {
            get { return bounds.X; }
            set { bounds.X = value; }
        }

        public int Y
        {
            get { return bounds.Y; }
            set { bounds.Y = value; }
        }

        public int Width
        {
            get { return bounds.Width; }
            set { bounds.Width = value; }
        }

        public int Height
        {
            get { return bounds.Height; }
            set { bounds.Height = value; }
        }

        protected int DoubleClickSpeed { get; set; }

        public DockItemGroup Owner { get; set; }

        public int Index { get; set; }

        public DateTime AddTime { get; set; }

        public bool WithinContainerBounds { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (Equals(name, value)) return;
                name = value;
                OnNameChanged(this, new EventArgs());
            }

        }

        public Padding Margin { get; set; }

        public Image Indicator
        {
            get { return indicator; }
        }

        public DockItemStyle Style
        {
            get { return Style; }
            set
            {
                if (Equals(Style, value)) return;
                style = value;
                OnStyleChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Private fields
        bool active;
        Rectangle bounds;
        bool mouseEntered;
        DateTime previousClick;
        Image indicator = Image.FromFile(@"C:\Users\William\Documents\Visual Studio 2010\Projects\DockResources\Icons\indicator.png");
        Image image;
        string name;
        private DockItemStyle style;
        #endregion

        protected DockItem()
        {
            mouseEntered = false;
            previousClick = DateTime.MinValue;
            DoubleClickSpeed = 500;

            Margin = new Padding(4);
        }

        public virtual void AddRightClickMenuItems(RightClickMenu rightClickMenu)
        {

        }

        protected static Image CreateReflection(Image image)
        {
            var reflected = new Bitmap(image);
            reflected.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var reflectedTransparent = new Bitmap(image.Width, image.Height);

            using (var g = Graphics.FromImage(reflectedTransparent))
            {
                var matrix = new ColorMatrix { Matrix33 = 0.3F };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(reflected, new Rectangle(new Point(0, 0), image.Size), 0, 0, image.Width, image.Height,
                            GraphicsUnit.Pixel, attributes);
            }

            return reflectedTransparent;
        }

        #region Mouse event handlers
        public bool HandleMouseClickEvent(object sender, MouseEventArgs e)
        {
            if (Bounds.Contains(e.Location))
            {
                OnMouseClick(sender, e);
                return true;
            }

            return false;
        }

        internal bool HandleMouseDoubleClickEvent(object sender, MouseEventArgs e)
        {
            if (Bounds.Contains(e.Location))
            {
                OnMouseDoubleClick(sender, e);
                return true;
            }

            return false;
        }

        public bool HandleMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (Bounds.Contains(e.Location))
            {
                if (!mouseEntered)
                {
                    OnMouseEnter(sender, new EventArgs());
                }
                mouseEntered = true;

                return true;
            }

            if (mouseEntered)
            {
                OnMouseLeave(sender, new EventArgs());
            }
            mouseEntered = false;

            return false;
        }

        public bool HandleMouseHoverEvent(object sender, MouseEventArgs e)
        {
            if (Bounds.Contains(e.Location))
            {
                MouseHover(sender, e);
                return true;
            }

            return false;
        }
        #endregion

        #region Drop event handlers
        /// <summary>
        /// Called by the owning dock to determine if an item is capable of handling a set of URIs on a drop.
        /// </summary>
        /// <param name="uris">
        /// A <see cref="IEnumerable<System.String>"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public virtual bool CanAcceptDrop(IEnumerable<string> uris)
        {
            return OnCanAcceptDrop(uris);
        }

        /// <summary>
        /// Called by the owning dock when a set of URIs has been dropped on an item and CanAcceptDrop has returned true
        /// </summary>
        /// <param name="uris">
        /// A <see cref="IEnumerable<System.String>"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public virtual bool AcceptDrop(IEnumerable<string> uris)
        {
            return OnAcceptDrop(uris);
        }

        /// <summary>
        /// Called by the owning dock to determine if an item is capable of handling an AbstractDockItem on a drop event.
        /// </summary>
        /// <param name="item">
        /// A <see cref="AbstractDockItem"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public virtual bool CanAcceptDrop(DockItem item)
        {
            return OnCanAcceptDrop(item);
        }

        /// <summary>
        /// Called by the owning dock when an AbstractDockitem has been dropped on an item and CanAcceptDrop has returned true
        /// </summary>
        /// <param name="item">
        /// A <see cref="DockItem"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public virtual bool AcceptDrop(DockItem item)
        {
            return OnAcceptDrop(item);
        }

        protected virtual bool OnCanAcceptDrop(IEnumerable<string> uris)
        {
            return false;
        }

        protected virtual bool OnAcceptDrop(IEnumerable<string> uris)
        {
            return false;
        }

        protected virtual bool OnCanAcceptDrop(DockItem item)
        {
            return false;
        }

        protected virtual bool OnAcceptDrop(DockItem item)
        {
            return false;
        }

        #endregion

        #region Event Callbacks
        protected virtual void OnStyleChanged(object sender, EventArgs e)
        {
            PaintRequested(sender, e);
        }

        protected virtual void OnMouseClick(object sender, MouseEventArgs e)
        {
            MouseClick(sender, e);
        }

        protected virtual void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseDoubleClick(sender, e);
        }

        protected virtual void OnImageChange(Image newImage)
        {
            PaintRequested(this, new EventArgs());
        }

        protected virtual void OnMouseEnter(object sender, EventArgs e)
        {
            MouseEnter(sender, e);
        }

        protected virtual void OnMouseLeave(object sender, EventArgs e)
        {
            MouseLeave(sender, e);
        }

        protected virtual void OnRequestPaint()
        {
            PaintRequested(this, new EventArgs());
        }

        protected virtual void OnNameChanged(object sender, EventArgs e)
        {
            NameChanged(sender, e);
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {

        }
        #endregion
    }
}