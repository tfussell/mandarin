using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Mandarin.Business.Core;

namespace Mandarin.Business.Plugins.Clock
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

        private TimeSpan offset;

        public ClockIcon()
        {
            try
            {
                var networkTime = GetNetworkTime();
                var localTime = TimeZone.CurrentTimeZone.ToLocalTime(networkTime);
                offset = DateTime.Now.Subtract(localTime);
            }
            catch
            {
                offset = new TimeSpan(0);
            }

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
                graphics.FillEllipse(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), new Rectangle(0, 0, 512, 512));
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                
                graphics.DrawString(DateTime.Now.Add(offset).ToShortTimeString(), new Font(FontFamily.GenericSansSerif, 40), Brushes.WhiteSmoke, new Rectangle(0, 0, 512, 512), stringFormat);
            }

            Image = iconImage;
        }

        void OpenControlPanelDateTime()
        {
            Process.Start("control", "/name Microsoft.DateAndTime");
        }

        public static DateTime GetNetworkTime()
        {
            const string ntpServer = "pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }
    }
}
