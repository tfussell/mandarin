using System;
using WinDock.Configuration;

namespace WinDock.Services
{
    class ServiceController : IDisposable
    {
        public ServiceConfiguration Configuration
        {
            get { return configuration; }
            set
            {
                if (Equals(configuration, value)) return;
                configuration = value;
                OnConfigurationChanged();
            }
        }

        private ServiceConfiguration configuration;

        public void Initialize(ServiceConfiguration config)
        {
            configuration = config;
        }

        private void OnConfigurationChanged()
        {

        }

        public void Dispose()
        {

        }
    }
}
