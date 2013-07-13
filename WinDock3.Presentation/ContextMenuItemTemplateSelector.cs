using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WinDock3.Business.ContextMenu;
using System.Windows;

namespace WinDock3.Presentation
{
    class ContextMenuItemTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var menuItem = (ContextMenuItem)item;
            if (menuItem != null)
            {
                if (menuItem is SeparatorContextMenuItem)
                {
                    return Application.Current.Resources["SeparatorTemplate"] as DataTemplate;
                }
                if (menuItem is TextContextMenuItem)
                {
                    return Application.Current.Resources["TextTemplate"] as DataTemplate;
                }
            }
            return null;
        }
    }
}
