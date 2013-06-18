using System.Drawing;
using WinDock.GUI;
using WindowsShellFacade;

namespace WinDock.Items
{
    class NotificationAreaIcon : DockItem
    {
        private NotificationAreaManaged notificationArea;

        public NotificationAreaIcon(DockWindow parent)
        {
            Image = new Bitmap(512, 512);
            notificationArea = new NotificationAreaManaged();
        }

        public void Update()
        {
            var count = 0;
            const int size = 512 / 3;

            using (var g = Graphics.FromImage(Image))
            {
                foreach (var icon in notificationArea.Icons)
                {
                    if (icon.IconImage != null)
                    {
                        g.DrawImage(icon.IconImage, X + size*(int) (count%3), Y + size*(int) (count/3), size, size);
                    }

                    count++;
                }
            }
        }
    }
}
