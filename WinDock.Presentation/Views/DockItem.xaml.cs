using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using WinDock.PresentationModel.ViewModels;

namespace WinDock.Presentation.Views
{
    /// <summary>
    /// Interaction logic for DockItem.xaml
    /// </summary>
    public partial class DockItem : UserControl
    {
        private DockContextMenu contextMenu;
        private DockItemToolTip toolTip;

        public DockItem()
        {
            InitializeComponent();
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var contextMenuModel = (DataContext as DockItemViewModel).ContextMenu;
                contextMenu = new DockContextMenu();
                contextMenu.DataContext = contextMenuModel;
                contextMenu.WindowStartupLocation = WindowStartupLocation.Manual;
                var position = PointToScreen(new Point(0, 0));
                contextMenu.Left = position.X + RenderSize.Width / 2 - contextMenu.RenderSize.Width / 2;
                contextMenu.Top = position.Y;
                contextMenu.Show();
                contextMenu.Left = position.X + RenderSize.Width / 2 - contextMenu.RenderSize.Width / 2;
                contextMenu.Top = position.Y - contextMenu.Height;
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (toolTip == null)
            {
                toolTip = new DockItemToolTip();
                toolTip.DataContext = DataContext;
                toolTip.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            var position = PointToScreen(new Point(0, 0));
            toolTip.Left = position.X + RenderSize.Width / 2 - toolTip.RenderSize.Width / 2;
            toolTip.Top = position.Y - toolTip.RenderSize.Height - 20;
            toolTip.ShowActivated = false;
            toolTip.Show();
            toolTip.Left = position.X + RenderSize.Width / 2 - toolTip.RenderSize.Width / 2;
            toolTip.Top = position.Y - toolTip.RenderSize.Height - 20;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.Hide();
            }
        }
    }
}
