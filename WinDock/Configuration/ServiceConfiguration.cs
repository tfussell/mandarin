namespace WinDock.Configuration
{
    /// <summary>
    /// Represents the set of configuration properties needed by any services.
    /// </summary>
    public class ServiceConfiguration : ConfigurationFile
    {
        /// <summary>
        /// Initializes a new instance of the ServiceConfiguration class from a given file.
        /// </summary>
        /// <param name="servicesConfigFile">The file to use for (de)serialization. 
        /// If it doesn't exist, it will be created and default values will be used during construction.</param>
        public ServiceConfiguration(string servicesConfigFile) : base(servicesConfigFile)
        {
            
        }

        /// <summary>
        /// Sets parameters relating to the ServiceConfiguration class to default values.
        /// </summary>
        protected override void SetDefault()
        {
            
        }
    }
}
