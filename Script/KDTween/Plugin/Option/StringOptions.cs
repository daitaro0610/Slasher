
namespace KD.Tweening.Plugin.Options
{
    public struct StringOptions : IPlugOptions
    {
        public bool richTextEnabled;
        public char[] scrambledChars;
        public ScrambleMode scrambleMode;

        public void Reset()
        {
            richTextEnabled = false;
            scrambledChars = null;
            scrambleMode = ScrambleMode.None;
        }
    }
}