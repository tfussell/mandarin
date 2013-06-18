using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WinDock.GUI;

namespace WinDock.Configuration
{
    /// <summary>
    /// Represents a window which may be used to view and modify various configuration properties.
    /// </summary>
    internal class ConfigurationWindow
    {
        /// <summary>
        /// Event handler instantiated by ActiveProfileChanged event.
        /// </summary>
        /// <param name="sender">The object which raised this event.</param>
        /// <param name="e">This parameter is always empty.</param>
        public delegate void ProfileChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// This event is called when ActiveProfile is changed.
        /// This allows listeners to update the application state in response to this change.
        /// </summary>
        public event ProfileChangedEventHandler ActiveProfileChanged;

        /// <summary>
        /// ConfigurationWindow singleton.
        /// ConfigurationWIndow may not be instantiated directly.
        /// This static variable provides a reference to the single window instance.
        /// </summary>
        public static readonly ConfigurationWindow Instance;

        /// <summary>
        /// Allows access to the currently loaded profile.
        /// Changes to this property will raise an ActiveProfileChanged event.
        /// </summary>
        public Profile ActiveProfile
        {
            get { return activeProfile; }
            set
            {
                if (!Equals(value, activeProfile))
                {
                    activeProfile = value;
                    OnActiveProfileChanged();
                }
            }
        }

        /// <summary>
        /// The set of possible profiles.
        /// </summary>
        public List<Profile> Profiles
        {
            get { return profiles; }
        }

        /// <summary>
        /// Form representing this window.
        /// </summary>
        private Form form;

        /// <summary>
        /// Field storing the set of possible profiles which may be activated.
        /// </summary>
        private readonly List<Profile> profiles;

        /// <summary>
        /// The profile which is currently activated by this window.
        /// </summary>
        private Profile activeProfile;

        /// <summary>
        /// Initializes singleton instance, Instance.
        /// </summary>
        static ConfigurationWindow()
        {
            Instance = new ConfigurationWindow();
        }

        /// <summary>
        /// Initializes a new ConfigurationWindow class, loading profiles from "ApplicationData"/profiles.
        /// </summary>
        private ConfigurationWindow()
        {
            profiles = new List<Profile>();
            var profilesDirectory = Path.Combine(ConfigurationFile.ApplicationDataFolder, "Profiles");
            foreach (var profileDirectory in Directory.GetDirectories(profilesDirectory))
            {
                var profileFile = Path.Combine(profileDirectory, "profile.config");
                profiles.Add(new Profile(profileFile));
            }
        }

        /// <summary>
        /// Displays this window. If the window is not intialized, it will be created automatically.
        /// </summary>
        public static void Show()
        {
            if (Instance.form == null)
            {
                Instance.BuildWindow();
                Instance.form.Show();
            }
            else if (!Instance.form.Visible)
            {
                Instance.BuildWindow();
                Instance.form.Show();
            }
            else
            {
                Instance.form.Activate();
            }
        }

        /// <summary>
        /// Hides this window. It will be destroyed automatically.
        /// </summary>
        public static void Hide()
        {
            Instance.form.Close();
            Instance.form = null;
        }

        /// <summary>
        /// Set up form with visual components allowing the configuration to by modified.
        /// </summary>
        private void BuildWindow()
        {
            form = new Form {AutoSize = true};

            var tp = new TabbedPanel(profiles);
            tp.ProfileChanged += active => ActiveProfile = active;
            form.Controls.Add(tp);

            form.MaximumSize = form.Size;
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Called when ActiveProfile has changed to raise corresponding event.
        /// </summary>
        private void OnActiveProfileChanged()
        {
            ActiveProfileChanged(this, new EventArgs());
        }
    }
}