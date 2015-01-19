using System.ComponentModel;
using System.IO;
using System.Web.Script.Serialization;

namespace Mandarin.Business.Settings
{
    public class ThemeConfiguration : INotifyPropertyChanged
    {
        public static ThemeConfiguration FromFile(string name)
        {
            string json = File.ReadAllText(Path.Combine(ConfigurationController.ApplicationDataFolder, "Themes", name));
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<ThemeConfiguration>(json);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
