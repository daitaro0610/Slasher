
namespace KD.Tweening.Plugin.Options
{
    public struct QuaternionOptions : IPlugOptions
    {
        public AxisConstraint axisConstraint;

        public void Reset()
        {
            axisConstraint = AxisConstraint.None;
        }
    }
}