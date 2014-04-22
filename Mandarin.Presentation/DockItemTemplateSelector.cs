using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using WinDock.PresentationModel.ViewModels;

namespace WinDock.Presentation
{
    class DockItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate DockItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (((DockItemViewModel)item).Model == null)
            {
                return SeparatorTemplate;
            }

            return DockItemTemplate;
        }
    }
}
