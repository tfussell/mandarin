using System;
using System.Collections.Generic;
using System.IO;
using WinDock.Configuration;

namespace WinDock.Themes
{
    class ThemeController
    {
        public static ThemeController Instance;

        public ThemeConfiguration Configuration;
        public List<Theme> AvailableThemes;
        public Theme ActiveTheme;

        static ThemeController()
        {
            Instance = new ThemeController("themes.config");
        }

        public ThemeController(string configFile)
        {
            try
            {
                Configuration = new ThemeConfiguration(configFile);
            }
            catch (Exception)
            {
                Configuration = ThemeConfiguration.Default;
            }

            AvailableThemes = new List<Theme>();

            foreach (var themeFile in Directory.GetFiles(Configuration.ThemesFolder))
            {
                AvailableThemes.Add(new Theme(themeFile));
            }
        }
    }
}
