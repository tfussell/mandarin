using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using WinDock3.Business.Items;
using WinDock3.Business.ItemGroups;

namespace WinDock3.Business.Plugins
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
            var internalTypes = Assembly.GetExecutingAssembly().GetTypes();
            var internalItemGroups = internalTypes.Where(t => t.IsSubclassOf(typeof(DockItemGroup)));

            knownPlugins = new Dictionary<string, Plugin>();

            foreach (var itemGroupType in internalItemGroups)
            {
                var plugin = new Plugin(itemGroupType);
                RegisterPlugin(itemGroupType.Name, plugin);
            }

            RegisterPlugin("RecycleBin", new Plugin(typeof(SingleDockItemGroup<RecycleBinIcon>)));
            RegisterPlugin("Separator", new Plugin(typeof(SingleDockItemGroup<SeparatorItem>)));

            const string PluginDirectory = @"C:\Users\William\Desktop\DockResources\Plugins";
            var pluginAssemblies = Directory.EnumerateFiles(PluginDirectory, "*.dll");

            foreach (var assemblyFile in pluginAssemblies)
            {
                try
                {
                    var plugin = new Plugin(assemblyFile);

                    if (plugin.Valid)
                    {
                        RegisterPlugin(plugin.Name, plugin);
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
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
