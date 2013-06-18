using System.Windows.Forms;

namespace WinDock.Drawing
{
    internal class Animator
    {
        public delegate void AnimatorStepDelegate(int frame);

        private readonly int millisecondsPerUpdate;
        private readonly Timer timer;
        protected readonly Animation Animation;
        private int millisecondsElapsed;

        protected Animator(float durationInSeconds, float updatesPerSecond)
        {
            millisecondsPerUpdate = (int) (1000/updatesPerSecond);
            millisecondsElapsed = 0;

            Animation = new Animation();
            Animation.Begin += (p) => AnimationBegin(p);
            Animation.End += (p) =>
                {
                    AnimationEnd(p);
                    Stop();
                };
            Animation.Step += (p) => AnimationStep(p);

            timer = new Timer {Interval = millisecondsPerUpdate};
            timer.Tick += (s, e) => Animation.DoStep();

            TotalSteps = (int) (1000*durationInSeconds/millisecondsPerUpdate);

            Animation.StartValue = 0;
            Animation.EndValue = TotalSteps;
            Animation.StepSize = 1;
        }

        protected int TotalSteps { get; private set; }

        protected bool Running
        {
            get { return timer.Enabled; }
            set { timer.Enabled = value; }
        }

        public event AnimatorStepDelegate AnimationBegin = delegate { };
        public event AnimatorStepDelegate AnimationStep = delegate { };
        public event AnimatorStepDelegate AnimationEnd = delegate { };

        public void Start()
        {
            if (!timer.Enabled)
                timer.Enabled = true;
        }

        private void Stop()
        {
            timer.Enabled = false;
        }

        protected void Reset()
        {
            Animation.Reset();
        }
    }
}