namespace WinDock.Items
{
    class SingleDockItemGroup : DockItemGroup
    {
        public DockItem Item { get; set; }

        public SingleDockItemGroup(DockItem item)
        {
            Item = item;
        }
    }
}
