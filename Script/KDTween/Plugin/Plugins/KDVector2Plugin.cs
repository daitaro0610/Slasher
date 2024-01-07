using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;
using Unity.Burst;

namespace KD.Tweening.Plugin
{
    public struct KDVector2Plugin : KDTweenPlugin<Vector2, VectorOptions>
    {
        public void Init(KDTweenBase<Vector2, VectorOptions> t)
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
        /// Vector2Œ^‚Ì•Ï“®
        /// </summary>
        /// <param name="t"></param>
        /// <param name="options"></param>
        /// <param name="isRelative"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="elapsed"></param>
        /// <param name="startValue"></param>
        /// <param name="changeValue"></param>
        /// <param name="duration"></param>

        public void EvaluateAndApply(KDTweenBase<Vector2, VectorOptions> t, VectorOptions options, bool isRelative, KDGetter<Vector2> getter, KDSetter<Vector2> setter, float elapsed, Vector2 startValue, Vector2 endValue, float duration)
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
            switch (options.axisConstraint)
            {
                case AxisConstraint.X:
                    Vector2 resX = getter();
                    resX.x = startValue.x + t.ChangeValue.x * easeVal;
                    if (options.snapping) resX.x = (float)Math.Round(resX.x);
                    setter(resX);
                    break;
                case AxisConstraint.Y:
                    Vector2 resY = getter();
                    resY.y = startValue.y + t.ChangeValue.y * easeVal;
                    if (options.snapping) resY.y = (float)Math.Round(resY.y);
                    setter(resY);
                    break;
                default:
                    startValue.x += t.ChangeValue.x * easeVal;
                    startValue.y += t.ChangeValue.y * easeVal;
                    if (options.snapping)
                    {
                        startValue.x = (float)Math.Round(startValue.x);
                        startValue.y = (float)Math.Round(startValue.y);
                    }
                    setter(startValue);
                    break;
            }
        }

        public void SetChangeValue(KDTweenBase<Vector2, VectorOptions> t, Vector2 startValue, Vector2 endValue)
        {
            t.ChangeValue = endValue - startValue;
        }

        public Vector2 SetRelativeEndValue(Vector2 startValue, Vector2 endValue)
        {
            return endValue + endValue - startValue;
        }
    }

}