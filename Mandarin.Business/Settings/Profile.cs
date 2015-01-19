using System.Collections.Generic;
using System.ComponentModel;

namespace Mandarin.Business.Settings
{
    public class Profile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 

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

        public bool StartAtLogin
        {
            get { return startAtLogin; }
            set
            {
                if (Equals(startAtLogin, value)) return;
                startAtLogin = value;
                OnPropertyChanged("StartAtLogin");
            }
        }

        public Dictionary<string, DockConfiguration>  Docks
        {
            get { return docks; }
            set
            {
                if (Equals(docks, value)) return;
                docks = value;
                OnPropertyChanged("Docks");
            }
        }

        private string name;
        private bool startAtLogin;
        private Dictionary<string, DockConfiguration> docks;        

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}