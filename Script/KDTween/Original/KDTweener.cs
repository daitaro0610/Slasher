using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening
{
    public static class KDTweener
    {
        /// <summary>
        /// floatå^ÇÃTweenèàóù
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static KDTween<float, FloatOptions> To(KDGetter<float> getter, KDSetter<float> setter, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                getter,
                setter,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                });
        }

        public static KDShakeTween<Vector3, ShakeOptions> ShakeTo(KDGetter<Vector3> getter, KDSetter<Vector3> setter, float strength, float vibrato, float duration, Vector3? randomOffset = null, bool fadeOut = true)
        {
            if (randomOffset == null)
                randomOffset = Vector3.right * 100;

            return KDTweenManager.Instance.SetUp(
                getter,
                setter,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.None,
                    fadeOut = fadeOut
                });
        }
    }
}