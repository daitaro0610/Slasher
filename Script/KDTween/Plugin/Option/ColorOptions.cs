namespace KD.Tweening.Plugin.Options
{
    public struct ColorOptions : IPlugOptions
    {
        public bool alphaOnly;

        public void Reset()
        {
            alphaOnly = false;
        }
    }
}
