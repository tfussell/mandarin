using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandarin.Business.Core
{
    public class DockItemAction
    {
        public string Label { get; set; }
        public Action Action { get; set; }
        public bool IsTogglable { get; set; }
        public bool ToggleState { get; set; }
        public bool HasSubmenu { get; set; }
        public IEnumerable<DockItemAction> Submenu { get; set; }

        public static DockItemAction CreateToggle(string label, Action enabledAction, Action disabledAction, bool toggleState)
        {
            var action = new DockItemAction
            {
                Label = label,
                ToggleState = toggleState
            };

            action.Action = () => action.Toggle(enabledAction, disabledAction);

            return action;
        }

        public static DockItemAction CreateSubMenu(string label, IEnumerable<DockItemAction> subMenu)
        {
            return new DockItemAction
            {
                Label = label,
                HasSubmenu = true,
                Submenu = subMenu
            };
        }

        public static DockItemAction CreateNormal(string label, Action action)
        {
            return new DockItemAction
            {
                Label = label,
                Action = action
            };
        }

        private void Toggle(Action enabled, Action disabled)
        {
            if (ToggleState) disabled();
            else enabled();
            ToggleState = !ToggleState;
        }
    }
}
