using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using WinDock.Business.Themes;

namespace WinDock.Business.Core
{
    /// <summary>
    /// An abstract class representing any bounded displayable item on a dock.
    /// </summary>
    public abstract class DockItem : INotifyPropertyChanged, IDisposable
    {
        #region Properties
        public abstract IEnumerable<DockItemAction> MenuItems { get; }

        public bool Active
        {
            get { return active; }
            set
            {
                if (value == active) return;
                active = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Active"));
            }
        }

        public Image Image
        {
            get { return image; }
            protected set
            {
                if (Equals(image, value)) return;
                image = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public Image ReflectionImage { get; protected set; }

        public Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                if (Equals(bounds, value)) return;
                bounds = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Size"));
            }
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
            set
            {
                if (Equals(bounds.Width, value)) return;
                bounds.Width = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Size"));
            }
        }

        public int Height
        {
            get { return bounds.Height; }
            set
            {
                if (Equals(bounds.Height, value)) return;
                bounds.Height = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Size"));
            }
        }

        public DockItemGroup Owner { get; set; }

        public int Index { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (Equals(name, value)) return;
                name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public DockItemStyle Style
        {
            get { return style; }
            set
            {
                if (Equals(style, value)) return;
                style = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Style"));
            }
        }
        #endregion

        #region Private fields
        private bool active;
        private Rectangle bounds;
        private Image image;
        private string name;
        private DockItemStyle style;
        #endregion

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
        public void OnLeftClick()
        {
            OnLeftClick(this, new EventArgs());
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
        /// A <see cref="bool"/>
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
        /// A <see cref="bool"/>
        /// </returns>
        public virtual bool AcceptDrop(IEnumerable<string> uris)
        {
            return OnAcceptDrop(uris);
        }

        /// <summary>
        /// Called by the owning dock to determine if an item is capable of handling an AbstractDockItem on a drop event.
        /// </summary>
        /// <param name="item">
        /// A <see cref="DockItem"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
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
        /// A <see cref="bool"/>
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
        protected virtual void OnLeftClick(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {

        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}