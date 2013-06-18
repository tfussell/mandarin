using System;
using System.Windows.Forms;
using WinDock.Configuration;
using WinDock.Dock;
using WinDock.Services;

namespace WinDock
{
    internal class WinDock : ApplicationContext
    {
        private Profile activeProfile;
        private readonly DockController dockController = new DockController();
        private readonly ServiceController serviceController = new ServiceController();

        private WinDock()
        {
            const string currentProfileFilename = "default";
            activeProfile = new Profile(currentProfileFilename);

            ConfigurationWindow.Instance.ActiveProfileChanged += ConfigurationOnActiveProfileChanged;

            serviceController.Initialize(activeProfile.Service);
            dockController.Initialize(activeProfile.Docks);

            dockController.AllDocksClosed += Quit;
            dockController.AboutButtonClick += ShowAboutWindow;
            dockController.ConfigurationButtonClick += ShowConfigurationWindow;

            ShowConfigurationWindow();
        }

        private void ConfigurationOnActiveProfileChanged(object sender, EventArgs eventArgs)
        {
            activeProfile = ConfigurationWindow.Instance.ActiveProfile;
            serviceController.Configuration = activeProfile.Service;
            dockController.Configuration = activeProfile.Docks;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            dockController.Dispose();
            serviceController.Dispose();
        }


        [STAThread]
        public static void Main()
        {
            Application.Run(new WinDock());
        }

        private static void ShowAboutWindow()
        {
        }

        private static void ShowConfigurationWindow()
        {
            ConfigurationWindow.Show();
        }

        private static void Quit()
        {
            Application.ExitThread();
        }
    }
}