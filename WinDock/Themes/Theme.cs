using System.IO;

namespace WinDock.Themes
{
    public class Theme
    {
        private DockItemStyle dockItemStyle;
        private DockStyle dockStyle;

        public Theme(string themeFolder)
        {
            dockStyle = new DockStyle(Path.Combine(themeFolder, "dock.theme"))
                {
                    DockItemStyle = new DockItemStyle(Path.Combine(themeFolder, "item.theme"))
                };
        }
    }
}