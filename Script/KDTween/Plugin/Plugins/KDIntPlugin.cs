using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening.Plugin
{
    public struct KDIntPlugin : KDTweenPlugin<int, NoOptions>
    {
        public void Init(KDTweenBase<int, NoOptions> t)
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
        public void EvaluateAndApply(KDTweenBase<int, NoOptions> t, NoOptions options, bool isRelative, KDGetter<int> getter, KDSetter<int> setter, float elapsed, int startValue, int endValue, float duration)
        {
            if (isRelative)
            {
                t.ChangeValue = endValue;
            }
            else
            {
                SetChangeValue(t, startValue, endValue);
            }

            setter((int)(startValue + t.ChangeValue * EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period)));
        }

        public void SetChangeValue(KDTweenBase<int, NoOptions> t, int startValue, int endValue)
        {
            t.ChangeValue = endValue - startValue;
        }

        public int SetRelativeEndValue(int startValue, int endValue)
        {
            return endValue + endValue - startValue;
        }
    }

}