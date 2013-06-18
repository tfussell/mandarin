using WinDock.GUI;

namespace WinDock.Items
{
    class SeparatorItem : DockItem
    {
        public SeparatorItem()
        {
            Image = System.Drawing.Image.FromFile("C:\\Users\\William\\Documents\\Visual Studio 2010\\Projects\\DockResources\\Icons\\separator.png");
            WithinContainerBounds = true;
        }
    }
}
