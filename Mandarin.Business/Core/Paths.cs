using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace WinDock.Business.Core
{
    public static class Paths
    {
        public static string AppName { get; set; }
        public static string ApplicationData { get; set; }
        public static string Docks { get; set; }
        public static string Plugins { get; set; }
        public static string Resources { get; set; }
        public static string System { get; set; }
        public static string SystemRoot { get; set; }
        public static string SystemIconFile { get; set; }
        public static string Themes { get; set; }

        static Paths()
        {
            AppName = "WinDock";
            ApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
            System = Environment.SystemDirectory;
            SystemRoot = Path.GetPathRoot(Environment.SystemDirectory);
            SystemIconFile = Path.Combine(System, "imageres.dll");
            Resources = @"C:\Users\William\Documents\Visual Studio 2010\Projects\WinDock\Resources";
            Docks = Path.Combine(ApplicationData, "Docks");
            Plugins = Path.Combine(ApplicationData, "Plugins");
            Themes = Path.Combine(ApplicationData, "Themes");

            if(!Directory.Exists(ApplicationData))
            {
                Directory.CreateDirectory(ApplicationData);
            }
        }
    }
}
