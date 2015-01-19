using System.Collections.Generic;
using System.ComponentModel;
using Mandarin.Business.Themes;
using Mandarin.Business.Core;
using Newtonsoft.Json;
using System.IO;

namespace Mandarin.Business.Settings
{
    public class DockConfiguration : INotifyPropertyChanged
    {
        public static DockConfiguration Default
        {
            get
            {
                return new DockConfiguration
                {
                    Name = "Dock",
                    Edge = ScreenEdge.Bottom,
                    ScreenIndex = 0,
                    ItemGroups = new List<string> { "StartMenu", null, "Applications", null, "RecycleBin", "Clock" },
                    Autohide = false,
                    Reserve = false,
                    ThemeName = "MountainLion",
                    Size = 80
                };
            }
        }

        public DockConfiguration()
        {
        }

        public static DockConfiguration FromFile(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                string json = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<DockConfiguration>(json);
                config.filename = filename;
                return config;
            }
        }

        public void Save()
        {
            var directory = Path.Combine(Paths.Docks, name);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                filename = Path.Combine(directory, "dock.json");
            }
            using (var writer = new StreamWriter(filename, false))
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                writer.Write(json);
            }
        }

        private string filename;
        private string name;
        private ScreenEdge edge;
        private int screenIndex;
        private List<string> itemGroups;
        private bool autohide;
        private bool reserve;
        private string themeName;
        private int size;

        public string Name
        {
            get { return name; }
            set
            {
                if (Equals(name, value)) return;
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public ScreenEdge Edge
        {
            get { return edge; }
            set
            {
                if (Equals(edge, value)) return;
                edge = value;
                OnPropertyChanged("Edge");
            }
        }

        public int ScreenIndex
        {
            get { return screenIndex; }
            set
            {
                if (Equals(screenIndex, value)) return;
                screenIndex = value;
                OnPropertyChanged("ScreenIndex");
            }
        }

        public List<string> ItemGroups
        {
            get { return itemGroups; }
            set
            {
                if (Equals(itemGroups, value)) return;
                itemGroups = value;
                OnPropertyChanged("ItemGroups");
            }
        }

        public bool Autohide
        {
            get { return autohide; }
            set
            {
                if (Equals(autohide, value)) return;
                autohide = value;
                OnPropertyChanged("Autohide");
            }
        }

        public bool Reserve
        {
            get { return reserve; }
            set
            {
                if (Equals(reserve, value)) return;
                reserve = value;
                OnPropertyChanged("Reserve");
            }
        }

        public string ThemeName
        {
            get { return themeName; }
            set
            {
                if (Equals(themeName, value)) return;
                themeName = value;
                OnPropertyChanged("ThemeName");
            }
        }

        public int Size
        {
            get { return size; }
            set
            {
                if (Equals(size, value)) return;
                size = value;
                OnPropertyChanged("Size");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}