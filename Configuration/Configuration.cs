using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.IO;
using System.Web.Script.Serialization;

namespace WinDock
{
    class Configuration
    {
        // Misc
        public static int DefaultScreen { get; set; }

        // Paths
        public static String IconsFolder { get; set; }
        public static String TaskbarPinnedDirectory { get; set; }
        public static String RunningIndicatorFilename { get; set; }
        public static String SeparatorImageFilename { get; set; }
        public static String RecycleBinImageFilenameE { get; set; }
        public static String RecycleBinImageFilenameF { get; set; }

        // Canvas
        public static int CanvasHeight { get; set; }
        public static int CanvasWidth { get; set; }

        // Dock
        public static int DockHeight { get; set; }
        public static int DockSideSlope { get; set; }
        public static double DockBackgroundAlpha { get; set; }
        public static Color DockBackgroundColor { get; set; }

        // Icons
        public static int IconSize { get; set; }
        public static int IconMargin { get; set; }
        public static double IconMagnificationFactor { get; set; }

        static Configuration()
        {
            String app_data = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String config_filename = app_data + Path.DirectorySeparatorChar + "WinDock" + Path.DirectorySeparatorChar + "config.json";

            if (!File.Exists(config_filename))
            {
                DefaultScreen = 1;
                TaskbarPinnedDirectory = "\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar";
                IconsFolder = "C:\\Users\\William\\Desktop\\iconsFolder";
                SeparatorImageFilename = "separator.png";
                RecycleBinImageFilenameE = "Trash_Empty.png";
                RecycleBinImageFilenameF = "Trash_Full.png";
                CanvasHeight = 200;
                CanvasWidth = 1280;
                DockHeight = 45;
                DockSideSlope = 25;
                DockBackgroundAlpha = 0.9;
                DockBackgroundColor = Color.FromArgb(120, 120, 120);
                IconSize = 50;
                IconMargin = 6;

                Stream stream = new FileStream(config_filename, FileMode.CreateNew, FileAccess.Write);
                Save(stream);
                stream.Close();
            }
            else
            {
                Stream stream = new FileStream(config_filename, FileMode.Open, FileAccess.Read);
                Load(stream);
                stream.Close();
            }
        }

        public static void Load(Stream filestream)
        {
            StreamReader reader = new StreamReader(filestream);
            String json_string = reader.ReadToEnd();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<String, String> json = serializer.Deserialize<Dictionary<String, String>>(json_string);

            System.Reflection.PropertyInfo[] properties = typeof(Configuration).GetProperties();

            foreach(System.Reflection.PropertyInfo p in properties)
            {
                object value = null;
                Type t = p.PropertyType;

                if (t == typeof(Int32))
                {
                    value = Convert.ToInt32(json[p.Name]);
                }
                else if (t == typeof(String))
                {
                    value = json[p.Name];
                }
                else if (t == typeof(Color))
                {
                    int a = Convert.ToInt32(json[p.Name].Substring(json[p.Name].IndexOf("A=") + 2, 3));
                    int r = Convert.ToInt32(json[p.Name].Substring(json[p.Name].IndexOf("R=") + 2, 3));
                    int g = Convert.ToInt32(json[p.Name].Substring(json[p.Name].IndexOf("G=") + 2, 3));
                    int b = Convert.ToInt32(json[p.Name].Substring(json[p.Name].IndexOf("B=") + 2, 3));
                    value = Color.FromArgb(a, r, g, b);
                }
                else if (t == typeof(Double))
                {
                    value = Convert.ToDouble(json[p.Name]);
                }

                if(value != null)
                    p.SetValue(null, value, null);
            }
        }

        public static void Save(Stream filestream)
        {
            StreamWriter writer = new StreamWriter(filestream);

            writer.WriteLine("{");
            System.Reflection.PropertyInfo[] properties = typeof(Configuration).GetProperties();

            foreach (System.Reflection.PropertyInfo p in properties)
            {
                writer.WriteLine("\"" + p.Name + "\":" + p.GetValue(null, null).ToString() + (p != properties[properties.Length - 1] ? "," : ""));
            }

            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
        }
    }
}
