using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using WinDock.Business.Core;
using System.IO;
using System.Linq;
using WinDock.Plugins.Applications;
using System.Drawing;
using WinDock.Plugins.RecycleBin;

namespace WinDock.PresentationModel.ViewModels
{
    public class DockViewModel : ViewModelBase
    {
        public const string ItemsPropertyName = "Items";
        public const string IsDirtyPropertyName = "IsDirty";

        private ObservableCollection<DockItemViewModel> items;
        private bool isDirty;

        public ObservableCollection<DockItemViewModel> Items
        {
            get { return items; }
            set
            {
                if (Equals(items, value)) return;
                items = value;
                RaisePropertyChanged(ItemsPropertyName);
            }
        }

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty = value) return;
                isDirty = value;
                RaisePropertyChanged(IsDirtyPropertyName);
            }
        }

        public Dock Model
        {
            get;
            private set;
        }

        public DockViewModel()
        {
            if (!IsInDesignModeStatic)
            {
                throw new Exception();
            }

            items = new ObservableCollection<DockItemViewModel>
            {
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - OneNote.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - Excel.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - Word.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - Outlook.png")) })),
                new DockItemViewModel(null),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - PowerPoint.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - Publisher.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.ResourceDirectory, "IconImages", "Microsoft Office - Visio.png")) }))
            };

            
        }

        public DockViewModel(Dock model)
        {
            Model = model;
            Model.PropertyChanged += (s, e) =>
            {
                IsDirty = true;
            };

            if (IsInDesignModeStatic)
            {
                throw new Exception();
            }

            var viewModels = model.AllItems.Select(i => new DockItemViewModel(i));
            Items = new ObservableCollection<DockItemViewModel>(viewModels);
        }
    }
}
