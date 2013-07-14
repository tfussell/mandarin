using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using WinDock.Business.ContextMenu;

namespace WinDock.PresentationModel.ViewModels
{
    public class DockContextMenuViewModel : ViewModelBase
    {
        public const string ItemsPropertyName = "Items";

        public ObservableCollection<ContextMenuItem> Items
        {
            get { return items; }
            set
            {
                if (Equals(items, value)) return;
                items = value;
                RaisePropertyChanged(ItemsPropertyName);
            }
        }

        public ContextMenu Model
        {
            get;
            private set;
        }

        private ObservableCollection<ContextMenuItem> items;

        public DockContextMenuViewModel()
        {
            if (IsInDesignModeStatic)
            {
                Items = new ObservableCollection<ContextMenuItem>()
                {
                    new TextContextMenuItem("Context Menu 1", null),
                    new ToggleContextMenuItem("Context Menu 2", null, null, false),
                    new TextContextMenuItem("Context Menu 3", null),
                    new SeparatorContextMenuItem(),
                    new TextContextMenuItem("Context Menu 4", null)
                };
            }
            else
            {
                throw new Exception();
            }
        }

        public DockContextMenuViewModel(ContextMenu model)
        {
            Model = model;
            Items = new ObservableCollection<ContextMenuItem>(model.MenuItems);
        }
    }
}
