using System;

namespace WinDock.Business.ContextMenu
{
    public class ToggleContextMenuItem : ContextMenuItem
    {
        public string Text { get; set; }
        public Action Action { get; set; }
        public bool IsChecked { get; set; }

        private readonly Action enabled;
        private readonly Action disabled;
        private bool currentState;

        public ToggleContextMenuItem(string text, Action enabled, Action disabled, bool initialState)
        {
            Text = text;
            Action = Toggle;

            this.enabled = enabled;
            this.disabled = disabled;
            currentState = initialState;
        }

        private void Toggle()
        {
            if (currentState)
            {
                disabled();
                currentState = false;
            }
            else
            {
                enabled();
                currentState = true;
            }
        }
    }
}
