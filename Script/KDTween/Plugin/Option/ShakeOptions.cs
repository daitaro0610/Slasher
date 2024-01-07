namespace KD.Tweening.Plugin.Options
{
    public struct ShakeOptions : IPlugOptions
    {
        public AxisConstraint axisConstraint;
        public bool fadeOut;

        public void Reset()
        {
            axisConstraint = AxisConstraint.None;
            fadeOut = false;
        }
    }

}