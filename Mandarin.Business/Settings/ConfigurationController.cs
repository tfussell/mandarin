using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;

namespace Mandarin.Business.Settings
{
    public class ConfigurationController
    {
        public static string ApplicationDataFolder;

        public AppSettings App;
        public UserAppSettings UserApp;
        public Profile ActiveProfile
        {
            get { return activeProfile; }
            set { if (Equals(activeProfile, value)) return;
                activeProfile = value;
                OnActiveProfileChanged(this, activeProfile);
            }
        }
        public List<Profile> Profiles;

        private Profile activeProfile;

        static ConfigurationController()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ApplicationDataFolder = Path.Combine(appData, "DOSk");
        }

        public ConfigurationController()
        {
            var s = new JavaScriptSerializer();
            var userConfigFilename = Path.Combine(ApplicationDataFolder, "settings.json");
            var json = File.ReadAllText(userConfigFilename).Trim(new[] { '\0' });
            UserApp = s.Deserialize<UserAppSettings>(json);

            var assemblyDirectory = "";// Path.GetDirectoryName(global::System.Windows.Forms.Application.ExecutablePath);
            Debug.Assert(assemblyDirectory != null, "Assembly directory must not be null.");
            var appConfigFilename = Path.Combine(assemblyDirectory, "settings.json");
            if (File.Exists(appConfigFilename))
            {
                json = File.ReadAllText(appConfigFilename).Trim(new[] {'\0'});
                App = s.Deserialize<AppSettings>(json);
            }
            else
            {
                App = AppSettings.Default;
            }

            var profileName = UserApp.ActiveProfile ?? "Default";
            var profileFilename = Path.Combine(ApplicationDataFolder, "Profiles", profileName, "profile.json");
            json = File.ReadAllText(profileFilename).Trim(new[] { '\0' });
            ActiveProfile = s.Deserialize<Profile>(json);

            Profiles = new List<Profile>();

            foreach (var profileDirectory in Directory.GetDirectories(Path.Combine(ApplicationDataFolder, "Profiles")))
            {
                profileFilename = Path.Combine(ApplicationDataFolder, "Profiles", profileDirectory, "profile.json");
                json = File.ReadAllText(profileFilename).Trim(new[] { '\0' });
                Profiles.Add(s.Deserialize<Profile>(json));
            }
        }

        protected virtual void OnActiveProfileChanged(object sender, Profile newProfile)
        {
            ActiveProfileChanged(sender, newProfile);
        }

        public event Action<object, Profile> ActiveProfileChanged = delegate { };
    }
}
