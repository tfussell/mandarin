using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WinDock3.Business.ItemGroups;
using WinDock3.Business.Items;
using WinDock3.Business.System;
using System.Windows;
using WinDock3.Business.Dock;
using System.IO;
using WinDock3.Presentation.DataService;

namespace WinDock3.Presentation.ViewModel
{
    class ViewModelProvider
    {
        public static IDataService DataService { get; set; }
        public static MainViewModel Main { get; set; }

        static ViewModelProvider()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                DataService = new DesignDataService.DesignDataService();
            }
            else
            {
                DataService = new DataService.DataService();
            }

            Main = new MainViewModel(DataService);
        }

        public static void Cleanup()
        {
            Main.Cleanup();
        }
    }
}
