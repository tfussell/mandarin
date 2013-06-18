using System.Drawing;
using WinDock.GUI;
using ScreenEdge = WinDock.Dock.ScreenEdge;

namespace WinDock.Drawing
{
    class AutohideAnimator : PositionAnimator
    {
        private bool Hiding { get; set; }
        private bool Showing { get; set; }

        public ScreenEdge Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private ScreenEdge direction;
        private readonly Point hiddenPosition;
        private readonly Point visiblePosition;

        public AutohideAnimator(DockWindow dock, float duration) : base(dock, duration, 50)
        {
            Hiding = false;
            Showing = false;

            direction = dock.Edge;

            if (direction == ScreenEdge.Left)
            {
                visiblePosition = new Point(dock.Screen.Bounds.Left, 0);
                hiddenPosition = new Point(dock.Screen.Bounds.Left - dock.Width, 0);
            }
            else if (direction == ScreenEdge.Top)
            {
                visiblePosition = new Point(dock.Screen.Bounds.Left, dock.Screen.Bounds.Top);
                hiddenPosition = new Point(dock.Screen.Bounds.Left, dock.Screen.Bounds.Top - dock.Height);
            }
            else if (direction == ScreenEdge.Right)
            {
                visiblePosition = new Point(dock.Screen.Bounds.Right - dock.Width, dock.Screen.Bounds.Top);
                hiddenPosition = new Point(dock.Screen.Bounds.Right, dock.Screen.Bounds.Top);
            }
            else // Bottom
            {
                visiblePosition = new Point(dock.Screen.Bounds.Left, dock.Screen.Bounds.Bottom - dock.Height);
                hiddenPosition = new Point(dock.Screen.Bounds.Left, dock.Screen.Bounds.Bottom);
            }

            AnimationBegin += AutohideAnimator_AnimationBegin;
            AnimationEnd += AutohideAnimator_AnimationEnd;

            Animation.StepSize = 2;
        }

        void AutohideAnimator_AnimationEnd(int frame)
        {
            if (Hiding && Subject.Visible)
            {
                Subject.Hide();
            }

            Hiding = false;
            Showing = false; 
        }

        void AutohideAnimator_AnimationBegin(int frame)
        {
            if (Showing && !Subject.Visible)
            { 
                Subject.Show(); 
            }
        }

        public void Show()
        {
            Showing = true;

            StartPosition = hiddenPosition;
            EndPosition = visiblePosition;

            if (!Running)
            {
                Reset();
                Running = true;
            }
        }

        public void Hide()
        {
            Hiding = true;

            StartPosition = visiblePosition;
            EndPosition = hiddenPosition;

            if (!Running)
            {
                Reset();
                Running = true;
            }
        }
    }
}
