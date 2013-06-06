using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AnotherDirect2DTest
{
    internal class DockItem
    {
        private Rectangle bounds;

        public DockItem()
        {
            Margin = new Padding(4);
        }

        public bool WithinContainerBounds { get; set; }
        public Image Image { get; set; }
        public Image ReflectionImage { get; set; }

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

        public Padding Margin { get; set; }

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

        public virtual void OnMouseClick(MouseEventArgs e)
        {
        }
    }
}