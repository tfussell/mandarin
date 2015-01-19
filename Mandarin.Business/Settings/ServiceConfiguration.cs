using System.ComponentModel;

namespace Mandarin.Business.Settings
{
    /// <summary>
    /// Represents the set of configuration properties needed by any services.
    /// </summary>
    public class ServiceConfiguration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
