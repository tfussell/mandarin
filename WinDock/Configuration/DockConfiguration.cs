using System;
using WinDock.Dock;

namespace WinDock.Configuration
{
    public class DockConfiguration : ConfigurationFile
    {
        public delegate void ConfigurationChangedDelegate(string property, object value);

        private string[] apps;
        private bool autohide;
        private ScreenEdge edge;
        private string name;
        private bool reserve;
        private int screenIndex;

        public DockConfiguration(string name) : base(name)
        {

        }

        protected override void SetDefault()
        {
            if (File == "default")
            {
                Name = "Default";
                Edge = ScreenEdge.Top;
                ScreenIndex = 1;
                Applications = null;
                Reserve = true;
                Autohide = true;
            }
            else
            {
                Name = "Other";
                Edge = ScreenEdge.Left;
                ScreenIndex = 1;
                Applications = null;
                Reserve = true;
                Autohide = true;
            }
        }

        private string Name
        {
            get { return name; }
            set
            {
                name = value;
                ConfigurationChanged("Name", name);
            }
        }

        public ScreenEdge Edge
        {
            get { return edge; }
            private set
            {
                edge = value;
                ConfigurationChanged("Edge", edge);
            }
        }

        public int ScreenIndex
        {
            get { return screenIndex; }
            private set
            {
                screenIndex = value;
                ConfigurationChanged("ScreenIndex", screenIndex);
            }
        }

        private String[] Applications
        {
            get { return apps; }
            set
            {
                apps = value;
                ConfigurationChanged("Applications", apps);
            }
        }

        public bool Reserve
        {
            get { return reserve; }
            private set
            {
                reserve = value;
                ConfigurationChanged("Reserve", reserve);
            }
        }

        private bool Autohide
        {
            get { return autohide; }
            set
            {
                autohide = value;
                ConfigurationChanged("Autohide", autohide);
            }
        }

        public event ConfigurationChangedDelegate ConfigurationChanged = delegate { };
    }
}