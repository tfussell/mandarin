using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinDock3.Business.ItemGroups;
using WinDock3.Business.Items;
using System.Reflection;
using System.IO;

namespace WinDock3.Business.Plugins
{
    public class Plugin
    {
        public string Name { get; set; }
        public Type ItemGroup { get; set; }
        public bool Valid { get { return ItemGroup != null; } }

        public Plugin(string assemblyFile)
        {
            try
            {
                var assembly = Assembly.LoadFile(assemblyFile);
                ItemGroup = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DockItemGroup))).SingleOrDefault();
            }
            catch { }

            if (Valid)
            {
                Name = Path.GetFileNameWithoutExtension(assemblyFile);
            }
        }

        public Plugin(Type itemGroup)
        {
            ItemGroup = itemGroup;
            Name = itemGroup.Name;
        }
    }
}
