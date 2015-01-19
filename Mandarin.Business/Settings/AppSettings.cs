namespace Mandarin.Business.Settings
{
    public class AppSettings
    {
        public string DefaultProfile { get; set; }

        public static AppSettings Default
        {
            get { return new AppSettings(); }
        }

        public AppSettings()
        {
            DefaultProfile = "Default";
        }
    }
}
