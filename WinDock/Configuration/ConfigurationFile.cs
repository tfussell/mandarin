using System;
using System.IO;

namespace WinDock.Configuration
{
    /// <summary>
    /// ConfigurationFile is an abstract class with only public fields 
    /// which stores a set of configuration options.
    /// </summary>
    /// <remarks>
    /// Child classes must implement SetDefault() which initializes fields
    ///  when the corresponding file is malformed or missing.
    /// </remarks>
    public abstract class ConfigurationFile
    {
        /// <summary>
        /// Static variable used by all ConfigurationFiles.
        /// This is the location of all persistent application settings, including profiles.
        /// </summary>
        public static string ApplicationDataFolder { get; private set; }

        /// <summary>
        /// Path of file with which this instance is associated. This file
        /// will be used for initialization and writing changes.
        /// </summary>
        protected string File { get; private set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        /// Initializes all static variables when called.
        static ConfigurationFile()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ApplicationDataFolder = appData + Path.DirectorySeparatorChar + "WinDock" + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Initializes a new instance of a ConfigurationFile class from a given file.
        /// </summary>
        /// <param name="file">The file to be used for (de)serialization of configuration properties. 
        /// If it doesn't exist, default values will be used.</param>
        protected ConfigurationFile(string file)
        {
            File = file;
            LoadFromFileOrSetDefault();
        }

        private void LoadFromFileOrSetDefault()
        {
            SetDefault();
        }

        /// <summary>
        /// Initializes fields
        /// when the corresponding file is malformed or missing.
        /// </summary>
        protected abstract void SetDefault();
    }
}