using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using WinDock.Business.Core;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace WinDock.PresentationModel.ViewModels
{
    public class DockItemViewModel : ViewModelBase
    {
        public const string IconImagePropertyName = "IconImage";
        public const string NamePropertyName = "Name";
        public const string WidthPropertyName = "Width";
        public const string HeightPropertyName = "Height";
        public const string ContextMenuPropertyName = "ContextMenu";

        private ImageSource iconImage;
        private string name;
        private int width;
        private int height;
        private IEnumerable<ContextMenuItemViewModel> contextMenu;

        public ICommand LeftClickCommand { get; set; }

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

        public IEnumerable<ContextMenuItemViewModel> ContextMenu
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
                IconImage = new BitmapImage(new Uri(Path.Combine(Paths.Resources, "IconImages", "Terminal.png")));
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
            if (model != null)
            {
                Model = model;

                model.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "Image")
                    {
                        IconImage = ImageToBitmapSource(model.Image);
                    }
                };

                Model = model;
                IconImage = ImageToBitmapSource(model.Image);
                Name = model.Name;
                Width = 60;
                Height = 60;
                ContextMenu = model.MenuItems.Select(m => new ContextMenuItemViewModel(m)).ToList();

                LeftClickCommand = new RelayCommand(() => Model.OnLeftClick());
            }
        }

        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        private static BitmapSource ImageToBitmapSource(Image source)
        {
            if (source == null) return null;

            BitmapImage bi = new BitmapImage();

            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            source.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();

            return bi;
        }
    }
}
