using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening.Plugin
{
    public struct KDVector3Plugin : KDTweenPlugin<Vector3, VectorOptions>
    {
        public void Init(KDTweenBase<Vector3, VectorOptions> t)
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
        /// Vector3Œ^‚Ì•Ï“®
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
        public void EvaluateAndApply(KDTweenBase<Vector3,  VectorOptions> t, VectorOptions options, bool isRelative, KDGetter<Vector3> getter, KDSetter<Vector3> setter, float elapsed, Vector3 startValue, Vector3 endValue, float duration)
        {
            if (isRelative)
            {
                t.ChangeValue = endValue;
            }
            else
            {
                SetChangeValue(t,startValue, endValue);
            }

            float easeVal = EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period);
            switch (options.axisConstraint)
            {
                case AxisConstraint.X:
                    Vector3 resX = getter();
                    resX.x = startValue.x + t.ChangeValue.x * easeVal;
                    if (options.snapping) resX.x = (float)Math.Round(resX.x);
                    setter(resX);
                    break;
                case AxisConstraint.Y:
                    Vector3 resY = getter();
                    resY.y = startValue.y + t.ChangeValue.y * easeVal;
                    if (options.snapping) resY.y = (float)Math.Round(resY.y);
                    setter(resY);
                    break;
                case AxisConstraint.Z:
                    Vector3 resZ = getter();
                    resZ.z = startValue.z + t.ChangeValue.z * easeVal;
                    if (options.snapping) resZ.z = (float)Math.Round(resZ.z);
                    setter(resZ);
                    break;
                default:
                    startValue.x += t.ChangeValue.x * easeVal;
                    startValue.y += t.ChangeValue.y * easeVal;
                    startValue.z += t.ChangeValue.z * easeVal;
                    if (options.snapping)
                    {
                        startValue.x = (float)Math.Round(startValue.x);
                        startValue.y = (float)Math.Round(startValue.y);
                        startValue.z = (float)Math.Round(startValue.z);
                    }
                    setter(startValue);
                    break;
            }
        }

        public void SetChangeValue(KDTweenBase<Vector3,VectorOptions> t,Vector3 startValue, Vector3 endValue)
        {
            t.ChangeValue = endValue - startValue;
        }

        public Vector3 SetRelativeEndValue(Vector3 startValue, Vector3 endValue)
        {
            return endValue + endValue - startValue;
        }
    }
}