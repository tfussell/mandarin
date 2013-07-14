using System;

namespace WinDock.Business.ContextMenu
{
    public class TextContextMenuItem : ContextMenuItem
    {
        public string Text { get; set; }
        public Action Action { get; set; }

        public TextContextMenuItem(string text, Action action)
        {
            Text = text;
            Action = action;
        }
    }
}
