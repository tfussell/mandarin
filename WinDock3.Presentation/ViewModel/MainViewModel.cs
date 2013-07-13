using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using WinDock3.Business.Dock;
using System.Windows;
using WinDock3.Presentation.DataService;
using GalaSoft.MvvmLight;

namespace WinDock3.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public const string CustomersPropertyName = "Docks";
        private ObservableCollection<DockViewModel> docks;

        public ObservableCollection<DockViewModel> Docks
        {
            get
            {
                return docks;
            }

            set
            {
                if (docks == value)
                {
                    return;
                }

                docks = value;
                RaisePropertyChanged(CustomersPropertyName);
            }
        }

        public const string IsBusyPropertyName = "IsBusy";
        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        public Action<DockViewModel> SaveDockCommand
        {
            get;
            private set;
        }

        public Action RefreshCommand
        {
            get;
            private set;
        }

        private IDataService _service;

        public MainViewModel(IDataService service)
        {
            _service = service;

            Refresh();

            SaveDockCommand = (dock) =>
            {
                if (!IsBusy && dock.IsDirty)
                {
                    IsBusy = true;

                    _service.SaveDock(dock, result =>
                    {
                        dock.IsDirty = false;
                        IsBusy = false;
                    });
                }
            };

            RefreshCommand = () => Refresh();
        }

        private void Refresh()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            _service.GetDocks((docks, error) =>
            {
                if (error != null)
                {
                    // Display error, normally this would be done through a property
                    MessageBox.Show(error.Message);
                    return;
                }

                Docks = new ObservableCollection<DockViewModel>(docks);
                IsBusy = false;
            });
        }
    }
}
