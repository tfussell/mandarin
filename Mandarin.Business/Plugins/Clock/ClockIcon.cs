using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WinDock.Business.Core;

namespace WinDock.Business.Plugins.Clock
{
    class ClockIcon : DockItem
    {
        public override IEnumerable<DockItemAction> MenuItems
        {
            get
            {
                return new List<DockItemAction>
                {
                    DockItemAction.CreateNormal("Adjust Date/Time", OpenControlPanelDateTime)
                };
            }
        }

        public ClockIcon()
        {
            Name = "Clock";
            Update();

            var aTimer = new Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler((a,b) => Update());
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        void Update()
        {
            var iconImage = new Bitmap(512, 512);

            using (var graphics = Graphics.FromImage(iconImage))
            {
                graphics.FillEllipse(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), new Rectangle(0, 0, 512, 512));
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                graphics.DrawString(DateTime.Now.ToLocalTime().ToShortTimeString(), new Font(FontFamily.GenericSansSerif, 40), Brushes.WhiteSmoke, new Rectangle(0, 0, 512, 512), stringFormat);
            }

            Image = iconImage;
        }

        void OpenControlPanelDateTime()
        {
            Process.Start("control", "/name Microsoft.DateAndTime");
        }
    }
}
