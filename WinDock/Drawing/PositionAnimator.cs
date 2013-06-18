using System.Drawing;
using System.Windows.Forms;

namespace WinDock.Drawing
{
    internal class PositionAnimator : Animator
    {
        protected readonly Control Subject;

        protected PositionAnimator(Control subject, float duration, float updatesPerSecond)
            : base(duration, updatesPerSecond)
        {
            Subject = subject;

            AnimationBegin += PositionAnimator_AnimationBegin;
            AnimationEnd += PositionAnimator_AnimationEnd;
            AnimationStep += PositionAnimator_AnimationStep;
        }

        protected Point StartPosition { private get; set; }
        protected Point EndPosition { private get; set; }
        public Point CurrentPosition { get; set; }

        private void PositionAnimator_AnimationBegin(int frame)
        {
            Subject.Location = StartPosition;
        }

        private void PositionAnimator_AnimationEnd(int frame)
        {
            Subject.Location = EndPosition;
        }

        private void PositionAnimator_AnimationStep(int frame)
        {
            float percentComplete = frame*1F/TotalSteps;
            Point tweened = TweenPosition(percentComplete);
            Subject.Location = tweened;
        }

        private Point TweenPosition(float percentComplete)
        {
            var dx = (int) ((EndPosition.X - StartPosition.X)*percentComplete);
            var dy = (int) ((EndPosition.Y - StartPosition.Y)*percentComplete);
            return new Point(StartPosition.X + dx, StartPosition.Y + dy);
        }
    }
}