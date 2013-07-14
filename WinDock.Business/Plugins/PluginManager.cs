using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using WinDock.Business.Core;
using WinDock.Plugins.Applications;
using WinDock.Plugins.RecycleBin;

namespace WinDock.Business.Plugins
{
    public class PluginManager
    {
        public static Dictionary<string, Plugin> KnownPlugins
        {
            get { return knownPlugins; }
        }

        private static Dictionary<string, Plugin> knownPlugins;

        static PluginManager()
        {
            knownPlugins = new Dictionary<string, Plugin>
            {
                { "Applications", new Plugin(typeof(ApplicationIconGroup)) },
                { "RecycleBin", new Plugin(typeof(SingleDockItemGroup<RecycleBinIcon>)) }
            };

            /*
            var pluginFiles = Directory.EnumerateFiles(Paths.PluginDirectory, "*.dll");

            var pluginAssemblies = new List<Assembly>();
            foreach (var assemblyFile in pluginFiles)
            {
                var assembly = Assembly.LoadFile(assemblyFile);
                pluginAssemblies.Add(assembly);

                var plugin = new Plugin(assembly);

                if (plugin.Valid)
                {
                    RegisterPlugin(plugin.Name, plugin);
                }
            }
             */
        }

        public static void RegisterPlugin(string name, Plugin plugin)
        {
            knownPlugins[name] = plugin;
        }

        public static void UnregisterPlugin(string name)
        {
            knownPlugins.Remove(name);
        }
    }
}
