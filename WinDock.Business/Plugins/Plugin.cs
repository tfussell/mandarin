using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using WinDock.Business.Core;

namespace WinDock.Business.Plugins
{
    public class Plugin
    {
        public string Name { get; set; }
        public Type ItemGroup { get; set; }
        public bool Valid { get { return ItemGroup != null; } }

        public Plugin(Assembly assembly)
        {
            try
            {
                ItemGroup = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DockItemGroup))).SingleOrDefault();
            }
            catch { }

            if (Valid)
            {
                Name = Path.GetFileNameWithoutExtension(assembly.CodeBase);
            }
        }

        public Plugin(Type itemGroup)
        {
            if (!itemGroup.IsSubclassOf(typeof(DockItemGroup)))
            {
                throw new Exception("Parameter itemGroup must inherit from DockItemGroup.");
            }

            ItemGroup = itemGroup;
            Name = itemGroup.Name;
        }
    }
}
