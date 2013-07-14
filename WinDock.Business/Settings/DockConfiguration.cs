using System.Collections.Generic;
using System.ComponentModel;
using WinDock.Business.Themes;
using WinDock.Business.Core;

namespace WinDock.Business.Settings
{
    public class DockConfiguration : INotifyPropertyChanged
    {
        private string name;
        private ScreenEdge edge;
        private int screenIndex;
        private List<string> itemGroups;
        private bool autohide;
        private bool reserve;
        private Theme theme;
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
            get { return new List<string> {"Applications", null, "RecycleBin"}; }
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

        public Theme Theme
        {
            get { return theme; }
            set
            {
                if (Equals(theme, value)) return;
                theme = value;
                OnPropertyChanged("Theme");
            }
        }

        public string ThemeName
        {
            get { return themeName; }
            set
            {
                if (Equals(themeName, value)) return;
                themeName = value;
                theme = Theme.FromFile(value);
            }
        }

        private string themeName;

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