using System;
using System.Windows.Threading;
using WinDock.Presentation.Views;

namespace WinDock.Presentation
{
    public class DockPositioner
    {
        private readonly DockWindow window;
        private readonly DispatcherTimer timer;
        private bool hidden;

        public DockPositioner(DockWindow dockWindow)
        {
            window = dockWindow;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            hidden = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Hide(0);
            timer.Stop();
        }

        public void ResetTimer()
        {
            timer.Stop();
            timer.Start();
            if (hidden)
            {
                Show();
            }
        }

        public void StartAutohide()
        {
            timer.Start();
        }

        public void StopAutohide()
        {
            timer.Stop();
            Show();
        }

        private void Show()
        {
            window.Top = 564.0 + 36;
            hidden = false;
        }

        private void Hide(double duration)
        {
            window.Top = 669.5;
            hidden = true;
        }
    }
}
