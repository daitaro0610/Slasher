using UnityEngine;
using KD.Tweening.Plugin.Options;
using KD.Cinemachine;
namespace KD.Tweening
{
    /// <summary>
    /// VirtualCameraêÍópÇÃTweenExtensionÉNÉâÉX
    /// </summary>
    public static class KDTweenVirtualCameraExtensions
    {
        public static KDTween<float, FloatOptions> TweenHorizontalAxis(this VirtualCamera target, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.Aim.m_HorizontalAxis.Value,
                (x) => target.Aim.m_HorizontalAxis.Value = x,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                }
                );
        }

        public static KDTween<float, FloatOptions> TweenVerticalAxis(this VirtualCamera target, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.Aim.m_VerticalAxis.Value,
                (x) => target.Aim.m_VerticalAxis.Value = x,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                }
                );
        }

        public static KDTween<float, FloatOptions> TweenDistance(this VirtualCamera target, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.Body.MaxDistance,
                (x) => target.Body.MaxDistance = x,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                }
                );
        }

        public static KDShakeTween<Vector3, ShakeOptions> ShakeCamera(this VirtualCamera target, float strength, float vibrato, float duration, Vector3? randomOffset= null)
        {
            if (randomOffset == null)
               randomOffset = KDShakeTweenExtensions.RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target.Body.Offset,
                (x) => target.Body.Offset = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.None,
                    fadeOut = true
                });
        }
    }
}