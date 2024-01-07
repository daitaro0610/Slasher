using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening.Plugin
{
    public struct KDColorPlugin : KDTweenPlugin<Color, ColorOptions>
    {
        public void Init(KDTweenBase<Color, ColorOptions> t)
        {
            if (t.IsRelative)
            {
                t.ChangeValue = t.EndValue;
            }
            else
            {
                SetChangeValue(t, t.StartValue, t.EndValue);
            }
        }

        public void EvaluateAndApply(KDTweenBase<Color, ColorOptions> t, ColorOptions options, bool isRelative, KDGetter<Color> getter, KDSetter<Color> setter, float elapsed, Color startValue, Color endValue, float duration)
        {
            if (isRelative)
            {
                t.ChangeValue = endValue;
            }
            else
            {
                SetChangeValue(t, startValue, endValue);
            }

            float easeVal = EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period);

            if (!options.alphaOnly)
            {
                startValue.r += t.ChangeValue.r * easeVal;
                startValue.g += t.ChangeValue.g * easeVal;
                startValue.b += t.ChangeValue.b * easeVal;
                startValue.a += t.ChangeValue.a * easeVal;
                setter(startValue);
            }
            else
            {
                startValue.a += t.ChangeValue.a * easeVal;
                setter(startValue);
            }
        }

        public void SetChangeValue(KDTweenBase<Color, ColorOptions> t, Color startValue, Color endValue)
        {
            t.ChangeValue = endValue - startValue;
        }

        public Color SetRelativeEndValue(Color startValue, Color endValue)
        {
            return endValue + endValue + startValue;
        }
    }
}