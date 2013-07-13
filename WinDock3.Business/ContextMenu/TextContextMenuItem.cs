using System;

namespace WinDock3.Business.ContextMenu
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
