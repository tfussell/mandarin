using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace WinDock.Business.Core
{
    public static class Paths
    {
        public static string AssemblyName { get; set; }
        public static string ApplicationDataDirectory { get; set; }
        public static string PluginDirectory { get; set; }
        public static string ResourceDirectory { get; set; }
        public static string SystemDirectory { get; set; }
        public static string SystemRoot { get; set; }
        public static string SystemIconFile { get; set; }

        static Paths()
        {
            AssemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Name;
            ApplicationDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AssemblyName);
            SystemDirectory = Environment.SystemDirectory;
            SystemRoot = Path.GetPathRoot(Environment.SystemDirectory);
            SystemIconFile = Path.Combine(SystemDirectory, "imageres.dll");
            ResourceDirectory = @"C:\Users\William\Documents\Visual Studio 2010\Projects\WinDock\Resources";
            PluginDirectory = Path.Combine(ResourceDirectory, "Plugins");
        }
    }
}
