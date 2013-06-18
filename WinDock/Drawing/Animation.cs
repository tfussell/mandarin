namespace WinDock.Drawing
{
    internal enum LoopType
    {
        ForwardStop,
        FromBeginningLoop,
        ForwardReverseStop,
        ForwardReverseLoop
    }

    internal class Animation
    {
        public delegate void AnimationStepDelegate(int currentValue);

        public int StartValue { private get; set; }
        public int EndValue { private get; set; }
        private int CurrentValue { get; set; }
        public int StepSize { private get; set; }
        private uint Repeats { get; set; }
        private LoopType LoopType { get; set; }
        public event AnimationStepDelegate Begin = delegate { };
        public event AnimationStepDelegate Step = delegate { };
        public event AnimationStepDelegate End = delegate { };

        public Animation()
        {
            StartValue = 0;
            EndValue = 0;
            CurrentValue = 0;
            StepSize = 1;
            Repeats = 0;
            LoopType = LoopType.ForwardStop;
        }

        public void DoStep()
        {
            if (CurrentValue == StartValue)
            {
                Begin(CurrentValue);
            }

            CurrentValue += StepSize;
            Step(CurrentValue);

            if (LoopType == LoopType.ForwardStop)
            {
                if ((StartValue < EndValue && CurrentValue >= EndValue) ||
                    (StartValue > EndValue && CurrentValue <= EndValue))
                {
                    if (Repeats > 1)
                    {
                        Repeats--;
                        CurrentValue = StartValue;
                    }
                    else
                    {
                        End(CurrentValue);
                    }
                }
            }
            else if (LoopType == LoopType.FromBeginningLoop)
            {
                if ((StartValue < EndValue && CurrentValue >= EndValue) ||
                    (StartValue > EndValue && CurrentValue <= EndValue))
                {
                    CurrentValue = StartValue;
                    CurrentValue -= StepSize;
                }
            }
            else if (LoopType == LoopType.ForwardReverseLoop)
            {
                if ((StartValue < EndValue && StepSize > 0 && CurrentValue >= EndValue) ||
                    (StartValue > EndValue && StepSize < 0 && CurrentValue <= EndValue) ||
                    (StartValue < EndValue && StepSize < 0 && CurrentValue <= StartValue) ||
                    (StartValue > EndValue && StepSize > 0 && CurrentValue >= StartValue))
                {
                    StepSize *= -1;
                }
            }
            else if (LoopType == LoopType.ForwardReverseStop)
            {
                if ((StartValue < EndValue && StepSize > 0 && CurrentValue >= EndValue) ||
                    (StartValue > EndValue && StepSize < 0 && CurrentValue <= EndValue))
                {
                    StepSize *= -1;
                }
                else if ((StartValue < EndValue && StepSize < 0 && CurrentValue <= StartValue) ||
                         (StartValue > EndValue && StepSize > 0 && CurrentValue >= StartValue))
                {
                    if (Repeats > 1)
                    {
                        Repeats--;
                        StepSize *= -1;
                    }
                    else
                    {
                        End(CurrentValue);
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentValue = StartValue;
            Step(CurrentValue);
        }
    }
}