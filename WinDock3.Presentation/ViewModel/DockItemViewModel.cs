using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using WinDock3.Business.ContextMenu;
using WinDock3.Business.Items;

namespace WinDock3.Presentation.ViewModel
{
    public class DockItemViewModel : ViewModelBase
    {
        public const string IconImagePropertyName = "Item";
        public const string NamePropertyName = "Name";
        public const string WidthPropertyName = "Width";
        public const string HeightPropertyName = "Height";
        public const string ContextMenuPropertyName = "ContextMenu";

        private ImageSource iconImage;
        private string name;
        private int width;
        private int height;
        private DockContextMenuViewModel contextMenu;

        public ImageSource IconImage
        {
            get { return iconImage; }
            set
            {
                if (Equals(iconImage, value)) return;
                iconImage = value;
                RaisePropertyChanged(IconImagePropertyName);
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (Equals(name, value)) return;
                name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                if (Equals(width, value)) return;
                width = value;
                RaisePropertyChanged(WidthPropertyName);
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (Equals(height, value)) return;
                height = value;
                RaisePropertyChanged(WidthPropertyName);
            }
        }

        public DockContextMenuViewModel ContextMenu
        {
            get { return contextMenu; }
            set
            {
                if (Equals(contextMenu, value)) return;
                contextMenu = value;
                RaisePropertyChanged(ContextMenuPropertyName);
            }
        }

        public DockItem Model
        {
            get;
            private set;
        }

        public DockItemViewModel()
        {
            if (IsInDesignModeStatic)
            {
                IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Terminal.png"));
                Name = "Terminal";
                Width = 60;
                Height = 60;
            }
            else
            {
                throw new Exception();
            }
        }

        public DockItemViewModel(DockItem model)
        {
            Model = model;
            model.PropertyChanged += (s, e) =>
            {
                
            };

            if (!IsInDesignModeStatic)
            {
                Model = model;
                IconImage = ImageToBitmapSource(model.Image);
                Name = model.Name;
                Width = 60;
                Height = 60;
                ContextMenu = new DockContextMenuViewModel(new ContextMenu(model));
            }
            else
            {
                throw new Exception();
            }
        }

        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        private static BitmapSource ImageToBitmapSource(System.Drawing.Image source)
        {
            if (source == null) return null;

            BitmapImage bi = new BitmapImage();

            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            source.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }
    }
}
