using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening.Plugin
{
    public struct KDQuaternionPlugin : KDTweenPlugin<Quaternion, QuaternionOptions>
    {
        public void Init(KDTweenBase<Quaternion, QuaternionOptions> t)
        {
        }

        public void EvaluateAndApply(KDTweenBase<Quaternion, QuaternionOptions> t, QuaternionOptions options, bool isRelative, KDGetter<Quaternion> getter, KDSetter<Quaternion> setter, float elapsed, Quaternion startValue, Quaternion endValue, float duration)
        {
        }

        public void SetChangeValue(KDTweenBase<Quaternion, QuaternionOptions> t, Quaternion startValue, Quaternion endValue)
        {
        }

        public Quaternion SetRelativeEndValue(Quaternion startValue, Quaternion endValue)
        {
            return endValue;
        }
    }
}