using System.IO;
using System.Web.Script.Serialization;
using Mandarin.Business.Settings;

namespace Mandarin.Business.Themes
{
    public class Theme
    {
        public DockItemStyle DockItemStyle;
        public DockStyle DockStyle;

        public static Theme FromFile(string themeDirectory)
        {
            var serializer = new JavaScriptSerializer();
            var themeFile = Path.Combine(ConfigurationController.ApplicationDataFolder, "Themes", themeDirectory, "theme.json");
            var json = File.ReadAllText(themeFile);
            var theme = serializer.Deserialize<Theme>(json);
            return theme;
        }
    }
}