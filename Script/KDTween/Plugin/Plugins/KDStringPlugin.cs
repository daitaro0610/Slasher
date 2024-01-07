using System;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;
using System.Text;

namespace KD.Tweening.Plugin
{
    public struct KDStringPlugin : KDTweenPlugin<string, StringOptions>
    {
        StringBuilder m_Builder;

        public void Init(KDTweenBase<string, StringOptions> t)
        {
            if (t.IsRelative)
            {
                t.ChangeValue = t.EndValue;
            }
            else
            {
                SetChangeValue(t, t.StartValue, t.EndValue);
            }

            m_Builder = new();
        }

        public void EvaluateAndApply(KDTweenBase<string, StringOptions> t, StringOptions options, bool isRelative, KDGetter<string> getter, KDSetter<string> setter, float elapsed, string startValue, string endValue, float duration)
        {
            m_Builder.Append(startValue);

            int startValueLength = string.IsNullOrEmpty(startValue) ? 0 : startValue.Length;
            int changeValueLength = t.ChangeValue.Length;

            int length = (int)Math.Round(changeValueLength * EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period));
            if (length > changeValueLength) length = changeValueLength;
            else if (length < 0) length = 0;


            EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period);
        }


        public void SetChangeValue(KDTweenBase<string, StringOptions> t, string startValue, string endValue)
        {
            t.ChangeValue = t.EndValue;
        }

        public string SetRelativeEndValue(string startValue, string endValue)
        {
            return endValue;
        }
    }
}