using System.Collections.Generic;
using System.IO;
using WinDock.Business.Settings;

namespace WinDock.Business.Themes
{
    class ThemeController
    {
        public List<Theme> Themes;

        public ThemeController()
        {
            var directory = Path.Combine(ConfigurationController.ApplicationDataFolder, "Themes");
            var themeDirectories = Directory.GetDirectories(directory);
            Themes = new List<Theme>();
            foreach (var themeDirectory in themeDirectories)
            {
                var theme = Theme.FromFile(themeDirectory);
                Themes.Add(theme);
            }
        }
    }
}
