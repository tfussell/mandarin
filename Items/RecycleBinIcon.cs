using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Shell32;

namespace WinDock
{
    class RecycleBinIcon : DockIcon
    {
        Bitmap empty = null;
        Bitmap full = null;
        Shell shell = null;

        public RecycleBinIcon()
        {
            empty = (Bitmap)Image.FromFile("C:\\Users\\William\\Desktop\\iconsFolder\\Trash_Empty.png");
            full = (Bitmap)Image.FromFile("C:\\Users\\William\\Desktop\\iconsFolder\\Trash_Full.png");

            Width = Configuration.IconSize;
            Height = Configuration.IconSize;

            Update(0);
        }

        public override void Update(int index)
        {
            base.Update(index);

            if(shell == null)
                shell = new Shell();

            bool is_empty = shell.NameSpace(Shell32.ShellSpecialFolderConstants.ssfBITBUCKET).Items().Count == 0;

            if (is_empty && Bitmap != empty)
            {
                Bitmap = empty;
            }
            else if (!is_empty && Bitmap != full)
            {
                Bitmap = full;
            }
        }
    }
}
