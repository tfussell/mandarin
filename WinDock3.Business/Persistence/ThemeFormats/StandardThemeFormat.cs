namespace WinDock3.Business.Persistence.ThemeFormats
{
    public class StandardThemeFormat : IThemeFormat
    {
        public static string IndicatorPath = "indicator.png";
        public static string IconImageDirectory = "Icons";
        public string BackgroundImage { get { return "Background.png"; } }
    }
}
