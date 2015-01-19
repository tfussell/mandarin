using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Mandarin.Business.Core;
using System.IO;
using System.Linq;
using Mandarin.Plugins.Applications;
using System.Drawing;
using Mandarin.Plugins.RecycleBin;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Mandarin.PresentationModel.ViewModels
{
    public class DockViewModel : ViewModelBase
    {
        public const string ItemsPropertyName = "Items";
        public const string IsDirtyPropertyName = "IsDirty";
        public const string ContextMenuPropertyName = "ContextMenu";
        public const string AutohidePropertyName = "Autohide";

        private IEnumerable<ContextMenuItemViewModel> contextMenu;
        private ObservableCollection<DockItemViewModel> items;
        private bool isDirty;
        private bool autohide;

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

        public bool Autohide
        {
            get { return autohide; }
            set
            {
                if (Equals(autohide, value)) return;
                autohide = value;
                RaisePropertyChanged(AutohidePropertyName);
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
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - OneNote.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - Excel.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - Word.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - Outlook.png")) })),
                new DockItemViewModel(null),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - PowerPoint.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - Publisher.png")) })),
                new DockItemViewModel(new ApplicationDockItem(new DesktopEntry { Icon = Bitmap.FromFile(Path.Combine(Paths.Resources, "IconImages", "Microsoft Office - Visio.png")) }))
            };
        }

        public DockViewModel(Dock model)
        {
            Model = model;
            Model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == AutohidePropertyName)
                {
                    Autohide = model.Configuration.Autohide;
                }
                IsDirty = true;
            };

            if (IsInDesignModeStatic)
            {
                throw new Exception();
            }

            var viewModels = model.AllItems.Select(i => new DockItemViewModel(i));
            Items = new ObservableCollection<DockItemViewModel>(viewModels);

            ContextMenu = new List<ContextMenuItemViewModel>
            {
                new ContextMenuItemViewModel(DockItemAction.CreateToggle("Autohide Dock", () => Model.Configuration.Autohide = true, () => Model.Configuration.Autohide = false, Model.Configuration.Autohide)),
                new ContextMenuItemViewModel(DockItemAction.CreateNormal("Close", CloseDockWindow))
            };

            
        }

        private void CloseDockWindow()
        {
            Model.OnClosed();
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "CloseDockWindow"));
        }
    }
}
