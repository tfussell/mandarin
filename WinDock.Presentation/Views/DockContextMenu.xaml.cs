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

namespace WinDock.Presentation.Views
{
    /// <summary>
    /// Interaction logic for DockContextMenu.xaml
    /// </summary>
    public partial class DockContextMenu
    {
        public DockContextMenu()
        {
            InitializeComponent();
        }

        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var item = (MenuItem)sender;
            

            var topRight = item.PointToScreen(new Point(Width, 0));
            var menu = new DockContextMenu();
            menu.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            menu.Left = topRight.X - 34;
            menu.Top = topRight.Y - 12;
            menu.Show();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }
    }
}
