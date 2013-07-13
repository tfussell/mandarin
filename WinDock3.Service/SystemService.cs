using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using WindowsManagedApi;

namespace WinDock3.Service
{
    public static class SystemService
    {
        public struct SystemIconIndices
        {
            public const int RecycleBinEmpty = 50;
            public const int RecycleBinFull = 49;
            public const int Directory = 4;
        }

        public static string AssemblyName { get; set; }
        public static string ApplicationDataDirectory { get; set; }
        public static string ApplicationDirectory { get; set; }
        public static string SystemDirectory { get; set; }
        public static string SystemRoot { get; set; }
        public static string SystemIconFile { get; set; }

        static SystemService()
        {
            AssemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Name;
            ApplicationDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AssemblyName);
            //ApplicationDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            SystemDirectory = Environment.SystemDirectory;
            SystemRoot = Path.GetPathRoot(Environment.SystemDirectory);
            SystemIconFile = Path.Combine(SystemDirectory, "imageres.dll");
        }

        public static Image GetSystemIcon(int index)
        {
            return User32.ExtractIconW(SystemIconFile, index);
        }
    }
}
