using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening.Plugin
{
    public struct KDFloatPlugin : KDTweenPlugin<float, FloatOptions>
    {
        public void Init(KDTweenBase<float, FloatOptions> t)
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

        /// <summary>
        /// FloatŒ^‚Ì’l‚Ì•Ï“®
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isRelative"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="elapsed"></param>
        /// <param name="startValue"></param>
        /// <param name="changeValue"></param>
        /// <param name="duration"></param>
        public void EvaluateAndApply(KDTweenBase<float, FloatOptions> t, FloatOptions options, bool isRelative, KDGetter<float> getter, KDSetter<float> setter, float elapsed, float startValue, float endValue, float duration)
        {
            if (isRelative)
            {
                t.ChangeValue = endValue;
            }
            else
            {
                SetChangeValue(t, startValue, endValue);
            }

            setter(
                 !options.snapping
                 ? startValue + t.ChangeValue * EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period)
                 : (float)Math.Round(startValue + t.ChangeValue * EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period))
             );
        }

        public void SetChangeValue(KDTweenBase<float, FloatOptions> t, float startValue, float endValue)
        {
            t.ChangeValue = endValue - startValue;
        }


        public float SetRelativeEndValue(float startValue, float endValue)
        {
            return endValue + endValue - startValue;
        }
    }
}