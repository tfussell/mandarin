using System.Collections.Generic;
using System.IO;

namespace WinDock.Configuration
{
    public class Profile : ConfigurationFile
    {

        private string dockConfigFile;
        private string serviceConfigFile;
        private string themeConfigFile;

        private readonly List<DockConfiguration> docks;
        private readonly ServiceConfiguration service;
        private readonly ThemeConfiguration theme;

        public Profile(string profileFile) : base(profileFile)
        {
            Name = profileFile;
            docks = new List<DockConfiguration> { new DockConfiguration(profileFile) };
            theme = new ThemeConfiguration(profileFile);
            service = new ServiceConfiguration(profileFile);
        }

        public IEnumerable<DockConfiguration> Docks
        {
            get { return docks; }
        }

        public ServiceConfiguration Service
        {
            get { return service; }
        }

        public ThemeConfiguration Theme
        {
            get { return theme; }
        }

        public string Name { get; set; }

        protected override void SetDefault()
        {
            var profileDirectory = Path.GetDirectoryName(File);
            if (profileDirectory == null) return;
            dockConfigFile = Path.Combine(profileDirectory, "dock.config");
            themeConfigFile = Path.Combine(profileDirectory, "theme.config");
            serviceConfigFile = Path.Combine(profileDirectory, "service.config");
        }
    }
}