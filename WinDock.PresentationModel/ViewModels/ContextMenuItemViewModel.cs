using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WinDock.Business.Core;

namespace WinDock.PresentationModel.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ContextMenuItemViewModel : ViewModelBase
    {
        public string Label { get; set; }
        public ICommand Action { get; set; }
        public bool IsTogglable { get; set; }
        public bool ToggleState { get; set; }
        public bool HasSubmenu { get; set; }
        public bool IsSeparator { get; set; }
        public IEnumerable<ContextMenuItemViewModel> Submenu { get; set; }

        public DockItemAction Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the MenuItemViewModel class.
        /// </summary>
        public ContextMenuItemViewModel(DockItemAction model)
        {
            if (model == null)
            {
                IsSeparator = true;
                return;
            }

            Model = model;
            Label = model.Label;
            if(model.Action != null)
                Action = new RelayCommand(model.Action);
            IsTogglable = model.IsTogglable;
            ToggleState = model.ToggleState;
            HasSubmenu = model.HasSubmenu;
            if (model.HasSubmenu)
            {
                Submenu = model.Submenu.Select(m => new ContextMenuItemViewModel(m));
            }
        }
    }
}