namespace WinDock3.Business.Settings
{
    public class UserAppSettings : AppSettings
    {
        public string ActiveProfile { get; set; }

        public UserAppSettings()
        {
            ActiveProfile = "Default";
        }
    }
}
