using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinDock
{
    class WinDock
    {
        [STAThread]
        public static void Main()
        {
            Application.Run(new DockWindow());
        }
    }
}
