using System.Windows;
using GalaSoft.MvvmLight.Threading;
using WinDock.PresentationModel.Locators;
using System;
using WinDock.Presentation.Views;
using WinDock.Services;
using WinDock.PresentationModel.ViewModels;

namespace WinDock.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var service = new DockService();
            service.GetDocks((docks, error) =>
            {
                if (error != null)
                {
                    throw new Exception("Problem loading docks.", error);
                }

                foreach (var dock in docks)
                {
                    var window = new DockWindow();
                    var dockViewModel = new DockViewModel(dock);
                    window.DataContext = dockViewModel;
                    window.Show();
                }
            });
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            ViewModelLocator.Cleanup();
        }
    }
}
