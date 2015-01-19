using System;
using System.Windows.Threading;
using Mandarin.Presentation.Controls;

namespace Mandarin.Presentation.Helpers
{
    public class DockPositioner
    {
        enum AnimationState
        {
            Hiding,
            Showing,
            Stopped
        }

        private readonly DockWindow window;
        private readonly DispatcherTimer hideTimer;
        private readonly DispatcherTimer animationTimer;
        private AnimationState currentAnimation;
        private bool hidden;

        public DockPositioner(DockWindow dockWindow)
        {
            window = dockWindow;
            hideTimer = new DispatcherTimer();
            hideTimer.Tick += new EventHandler(timer_Tick);
            hideTimer.Interval = new TimeSpan(0, 0, 1);
            animationTimer = new DispatcherTimer();
            animationTimer.Tick += new EventHandler(animation_Tick);
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            hidden = false;
        }

        private void animation_Tick(object sender, EventArgs e)
        {
            switch (currentAnimation)
            {
                case AnimationState.Hiding:
                    {
                        if (window.Top < 669.5)
                        {
                            window.Top += 3;
                        }
                        if (window.Top > 669)
                        {
                            window.Top = 669.5;
                            currentAnimation = AnimationState.Stopped;
                            animationTimer.Stop();
                            hidden = true;
                        }
                        break;
                    }
                case AnimationState.Showing:
                    {
                        if (window.Top > 600)
                        {
                            window.Top -= 3;
                        }
                        if (window.Top < 600)
                        {
                            window.Top = 600;
                            currentAnimation = AnimationState.Stopped;
                            animationTimer.Stop();
                            hidden = false;
                        }
                        break;
                    }
                case AnimationState.Stopped:
                    {
                        break;
                    }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Hide(0);
            hideTimer.Stop();
        }

        public void StartTimer()
        {
            if (hidden)
            {
                Show();
            }
            hideTimer.Start();
        }

        public void StopTimer()
        {
            if (hidden)
            {
                Show();
            }
            hideTimer.Stop();
        }

        public void StartAutohide()
        {
            hideTimer.Start();
        }

        public void StopAutohide()
        {
            hideTimer.Stop();
            Show();
        }

        private void Show()
        {
            currentAnimation = AnimationState.Showing;
            animationTimer.Start();
        }

        private void Hide(double duration)
        {
            currentAnimation = AnimationState.Hiding;
            animationTimer.Start();
        }
    }
}
