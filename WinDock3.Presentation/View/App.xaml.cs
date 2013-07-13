using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using WinDock3.Business;
using WinDock3.Presentation.ViewModel;
using WinDock3.Presentation.View;

namespace WinDock3.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            foreach (var dockViewModel in ViewModelProvider.Main.Docks)
            {
                var window = new DockWindow();
                window.DataContext = dockViewModel;
                window.Show();
            }
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            ViewModelProvider.Cleanup();
        }
    }
}
