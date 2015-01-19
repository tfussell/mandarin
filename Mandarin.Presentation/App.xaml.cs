using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Mandarin.PresentationModel.Locators;
using System;
using Mandarin.Presentation.Views;
using Mandarin.Services;
using Mandarin.PresentationModel.ViewModels;
using Mandarin.Presentation.Controls;

namespace Mandarin.Presentation
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
                    var window = new DockView();
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
