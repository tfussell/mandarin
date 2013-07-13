using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using WinDock3.Business.Dock;

namespace WinDock3.Presentation.ViewModel
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
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft OneNote", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - OneNote.png")) },
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft Excel", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - Excel.png")) },
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft Word", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - Word.png")) },
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft Outlook", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - Outlook.png")) },
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft Powerpoint", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - PowerPoint.png")) },
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft Publisher", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - Publisher.png")) },
                new DockItemViewModel { Height = 50, Width = 50, Name = "Microsoft Visio", IconImage = new BitmapImage(new Uri(@"C:\Users\William\Desktop\DockResources\IconImages\Microsoft Office - Visio.png")) },
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

            items = new ObservableCollection<DockItemViewModel>();
            foreach (var item in model.AllItems)
            {
                items.Add(new DockItemViewModel(item));
            }
        }
    }
}
