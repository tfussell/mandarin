using System;
using System.IO;

namespace WinDock.Configuration
{
    public class ThemeConfiguration
    {
        public static readonly ThemeConfiguration Default;

        public string ThemesFolder { get; private set; }

        static ThemeConfiguration()
        {
            Default = new ThemeConfiguration("theme.config");
        }

        public ThemeConfiguration(string themeConfigFile)
        {
            /*
            using (var fileStream = File.Open(themeConfigFile, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(fileStream);
                ThemesFolder = reader.ReadLine();
            }
             */
        }
    }
}
