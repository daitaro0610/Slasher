using System;
using UnityEngine;
using Unity.Burst;

namespace KD.Tweening.Core
{
    public static class EaseManager
    {
        #region Const

        const float PI_OVER2 = Mathf.PI * 0.5f;
        const float TWO_PI = Mathf.PI * 2;

        #endregion

        #region Manipulators
        
        /// <summary>
        /// Easingの処理
        /// </summary>
        /// <param name="easeType">EasingのType Tweenアニメーションが変わる</param>
        /// <param name="time">現在の遷移時間</param>
        /// <param name="duration">何秒で遷移しきるか</param>
        /// <param name="overshootOrAmplitude">オーバーシュートの量やエラスティック振幅 0.5~2.0の値</param>
        /// <param name="period">周期 0.3~0.6の値</param>
        /// <returns></returns>
        [BurstCompile]
        public static float Evaluate(Ease easeType, float time, float duration, float overshootOrAmplitude, float period)
        {
            return easeType switch
            {
                Ease.Linear     => Liner(time, duration, overshootOrAmplitude, period),
                Ease.InSine     => InSine(time, duration, overshootOrAmplitude, period),
                Ease.OutSine    => OutSine(time, duration, overshootOrAmplitude, period),
                Ease.InOutSine  => InOutSine(time, duration, overshootOrAmplitude, period),
                Ease.InQuad     => InQuad(time, duration, overshootOrAmplitude, period),
                Ease.OutQuad    => OutQuad(time, duration, overshootOrAmplitude, period),
                Ease.InOutQuad  => InOutQuad(time, duration, overshootOrAmplitude, period),
                Ease.InCubic    => InCubic(time, duration, overshootOrAmplitude, period),
                Ease.OutCubic   => OutCubic(time, duration, overshootOrAmplitude, period),
                Ease.InOutCubic => InOutCubic(time, duration, overshootOrAmplitude, period),
                Ease.InQuart    => InQuart(time, duration, overshootOrAmplitude, period),
                Ease.OutQuart   => OutQuart(time, duration, overshootOrAmplitude, period),
                Ease.InOutQuart => InOutQuart(time, duration, overshootOrAmplitude, period),
                Ease.InQuint    => InQuint(time, duration, overshootOrAmplitude, period),
                Ease.OutQuint   => OutQuint(time, duration, overshootOrAmplitude, period),
                Ease.InOutQuint => InOutQuint(time, duration, overshootOrAmplitude, period),
                Ease.InExpo     => InExpo(time, duration, overshootOrAmplitude, period),
                Ease.OutExpo    => OutExpo(time, duration, overshootOrAmplitude, period),
                Ease.InOutExpo  => InOutExpo(time, duration, overshootOrAmplitude, period),
                Ease.InCirc     => InCirc(time, duration, overshootOrAmplitude, period),
                Ease.OutCirc    => OutCirc(time, duration, overshootOrAmplitude, period),
                Ease.InOutCirc  => InOutCirc(time, duration, overshootOrAmplitude, period),
                Ease.InElastic  => InElastic(time, duration, overshootOrAmplitude, period),
                Ease.OutElastic => OutElastic(time, duration, overshootOrAmplitude, period),
                Ease.InOutElastic => InOutElastic(time, duration, overshootOrAmplitude, period),
                Ease.InBack => InBack(time, duration, overshootOrAmplitude, period),
                Ease.OutBack    => OutBack(time, duration, overshootOrAmplitude, period),
                Ease.InOutBack  => InOutBack(time, duration, overshootOrAmplitude, period),
                Ease.InBounce   => InBounce(time, duration, overshootOrAmplitude, period),
                Ease.OutBounce  => OutBounce(time, duration, overshootOrAmplitude, period),
                Ease.InOutBounce => InOutBounce(time, duration, overshootOrAmplitude, period),
                //defaultのEasingはLiner
                _ => Liner(time, duration, overshootOrAmplitude, period),
            };
        }
        public static EaseFunction ToEaseFunction(Ease ease)
        {
            return ease switch
            {
                Ease.Linear     => (float time, float duration, float overshootOrAmplitude, float period) => Liner(time, duration, overshootOrAmplitude, period),
                Ease.InSine     => (float time, float duration, float overshootOrAmplitude, float period) => InSine(time, duration, overshootOrAmplitude, period),
                Ease.OutSine    => (float time, float duration, float overshootOrAmplitude, float period) => OutSine(time, duration, overshootOrAmplitude, period),
                Ease.InOutSine  => (float time, float duration, float overshootOrAmplitude, float period) => InOutSine(time, duration, overshootOrAmplitude, period),
                Ease.InQuad     => (float time, float duration, float overshootOrAmplitude, float period) => InQuad(time, duration, overshootOrAmplitude, period),
                Ease.OutQuad    => (float time, float duration, float overshootOrAmplitude, float period) => OutQuad(time, duration, overshootOrAmplitude, period),
                Ease.InOutQuad  => (float time, float duration, float overshootOrAmplitude, float period) => InOutQuad(time, duration, overshootOrAmplitude, period),
                Ease.InCubic    => (float time, float duration, float overshootOrAmplitude, float period) => InCubic(time, duration, overshootOrAmplitude, period),
                Ease.OutCubic   => (float time, float duration, float overshootOrAmplitude, float period) => OutCubic(time, duration, overshootOrAmplitude, period),
                Ease.InOutCubic => (float time, float duration, float overshootOrAmplitude, float period) => InOutCubic(time, duration, overshootOrAmplitude, period),
                Ease.InQuart    => (float time, float duration, float overshootOrAmplitude, float period) => InQuart(time, duration, overshootOrAmplitude, period),
                Ease.OutQuart   => (float time, float duration, float overshootOrAmplitude, float period) => OutQuart(time, duration, overshootOrAmplitude, period),
                Ease.InOutQuart => (float time, float duration, float overshootOrAmplitude, float period) => InOutQuart(time, duration, overshootOrAmplitude, period),
                Ease.InQuint    => (float time, float duration, float overshootOrAmplitude, float period) => InQuint(time, duration, overshootOrAmplitude, period),
                Ease.OutQuint   => (float time, float duration, float overshootOrAmplitude, float period) => OutQuint(time, duration, overshootOrAmplitude, period),
                Ease.InOutQuint => (float time, float duration, float overshootOrAmplitude, float period) => InOutQuint(time, duration, overshootOrAmplitude, period),
                Ease.InExpo     => (float time, float duration, float overshootOrAmplitude, float period) => InExpo(time, duration, overshootOrAmplitude, period),
                Ease.OutExpo    => (float time, float duration, float overshootOrAmplitude, float period) => OutExpo(time, duration, overshootOrAmplitude, period),
                Ease.InOutExpo  => (float time, float duration, float overshootOrAmplitude, float period) => InOutExpo(time, duration, overshootOrAmplitude, period),
                Ease.InCirc     => (float time, float duration, float overshootOrAmplitude, float period) => InCirc(time, duration, overshootOrAmplitude, period),
                Ease.OutCirc    => (float time, float duration, float overshootOrAmplitude, float period) => OutCirc(time, duration, overshootOrAmplitude, period),
                Ease.InOutCirc  => (float time, float duration, float overshootOrAmplitude, float period) => InOutCirc(time, duration, overshootOrAmplitude, period),
                Ease.InElastic  => (float time, float duration, float overshootOrAmplitude, float period) => InElastic(time, duration, overshootOrAmplitude, period),
                Ease.OutElastic => (float time, float duration, float overshootOrAmplitude, float period) => OutElastic(time, duration, overshootOrAmplitude, period),
                Ease.InOutElastic => (float time, float duration, float overshootOrAmplitude, float period) => InOutElastic(time, duration, overshootOrAmplitude, period),
                Ease.InBack     => (float time, float duration, float overshootOrAmplitude, float period) => InBack(time, duration, overshootOrAmplitude, period),
                Ease.OutBack    => (float time, float duration, float overshootOrAmplitude, float period) => OutBack(time, duration, overshootOrAmplitude, period),
                Ease.InOutBack  => (float time, float duration, float overshootOrAmplitude, float period) => InOutBack(time, duration, overshootOrAmplitude, period),
                Ease.InBounce   => (float time, float duration, float overshootOrAmplitude, float period) => InBounce(time, duration, overshootOrAmplitude, period),
                Ease.OutBounce  => (float time, float duration, float overshootOrAmplitude, float period) => OutBounce(time, duration, overshootOrAmplitude, period),
                Ease.InOutBounce => (float time, float duration, float overshootOrAmplitude, float period) => InOutBounce(time, duration, overshootOrAmplitude, period),
                _ => (float time, float duration, float overshootOrAmplitude, float period) => Liner(time, duration, overshootOrAmplitude, period)  //defaultはLiner
            };
        }

        #endregion

        #region Easing

        [BurstCompile]
        public static float Liner(float time, float duration, float overshootOrAmplitude, float period)
        {
            return time / duration;
        }
        [BurstCompile]
        public static float InSine(float time, float duration, float overshootOrAmplitude, float period)
        {
            return -(float)Math.Cos(time / duration * PI_OVER2) + 1;
        }
        [BurstCompile]
        public static float OutSine(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (float)Math.Sin(time / duration * PI_OVER2);
        }
        [BurstCompile]
        public static float InOutSine(float time, float duration, float overshootOrAmplitude, float period)
        {
            return -0.5f * ((float)Math.Cos(Mathf.PI * time / duration) - 1);
        }
        [BurstCompile]
        public static float InQuad(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (time /= duration) * time;
        }
        [BurstCompile]
        public static float OutQuad(float time, float duration, float overshootOrAmplitude, float period)
        {
            return -(time /= duration) * (time - 2);
        }
        [BurstCompile]
        public static float InOutQuad(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time;
            return -0.5f * ((--time) * (time - 2) - 1);
        }
        [BurstCompile]
        public static float InCubic(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (time /= duration) * time * time;
        }
        [BurstCompile]
        public static float OutCubic(float time, float duration, float overshootOrAmplitude, float period)
        {
            return ((time = time / duration - 1) * time * time + 1);
        }
        [BurstCompile]
        public static float InOutCubic(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time;
            return 0.5f * ((time -= 2) * time * time + 2);
        }
        [BurstCompile]
        public static float InQuart(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (time /= duration) * time * time * time;
        }
        [BurstCompile]
        public static float OutQuart(float time, float duration, float overshootOrAmplitude, float period)
        {
            return -((time = time / duration - 1) * time * time * time - 1);
        }
        [BurstCompile]
        public static float InOutQuart(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time;
            return -0.5f * ((time -= 2) * time * time * time - 2);
        }
        [BurstCompile]
        public static float InQuint(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (time /= duration) * time * time * time * time;
        }
        [BurstCompile]
        public static float OutQuint(float time, float duration, float overshootOrAmplitude, float period)
        {
            return ((time = time / duration - 1) * time * time * time * time + 1);
        }
        [BurstCompile]
        public static float InOutQuint(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time * time;
            return 0.5f * ((time -= 2) * time * time * time * time + 2);
        }
        [BurstCompile]
        public static float InExpo(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (time == 0) ? 0 : (float)Math.Pow(2, 10 * (time / duration - 1));
        }
        [BurstCompile]
        public static float OutExpo(float time, float duration, float overshootOrAmplitude, float period)
        {
            if (time == duration) return 1;
            return (-(float)Math.Pow(2, -10 * time / duration) + 1);
        }
        [BurstCompile]
        public static float InOutExpo(float time, float duration, float overshootOrAmplitude, float period)
        {
            if (time == 0) return 0;
            if (time == duration) return 1;
            if ((time /= duration * 0.5f) < 1) return 0.5f * (float)Math.Pow(2, 10 * (time - 1));
            return 0.5f * (-(float)Math.Pow(2, -10 * --time) + 2);
        }
        [BurstCompile]
        public static float InCirc(float time, float duration, float overshootOrAmplitude, float period)
        {
            return -((float)Math.Sqrt(1 - (time /= duration) * time) - 1);
        }
        [BurstCompile]
        public static float OutCirc(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (float)Math.Sqrt(1 - (time = time / duration - 1) * time);
        }
        [BurstCompile]
        public static float InOutCirc(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration * 0.5f) < 1) return -0.5f * ((float)Math.Sqrt(1 - time * time) - 1);
            return 0.5f * ((float)Math.Sqrt(1 - (time -= 2) * time) + 1);
        }
        [BurstCompile]
        public static float InElastic(float time, float duration, float overshootOrAmplitude, float period)
        {
            float s0;
            if (time == 0) return 0;
            if ((time /= duration) == 1) return 1;
            if (period == 0) period = duration * 0.3f;
            if (overshootOrAmplitude < 1)
            {
                overshootOrAmplitude = 1;
                s0 = period / 4;
            }
            else s0 = period / TWO_PI * (float)Math.Asin(1 / overshootOrAmplitude);
            return -(overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * duration - s0) * TWO_PI / period));
        }
        [BurstCompile]
        public static float OutElastic(float time, float duration, float overshootOrAmplitude, float period)
        {
            float s1;
            if (time == 0) return 0;
            if ((time /= duration) == 1) return 1;
            if (period == 0) period = duration * 0.3f;
            if (overshootOrAmplitude < 1)
            {
                overshootOrAmplitude = 1;
                s1 = period / 4;
            }
            else s1 = period / TWO_PI * (float)Math.Asin(1 / overshootOrAmplitude);
            return (overshootOrAmplitude * (float)Math.Pow(2, -10 * time) * (float)Math.Sin((time * duration - s1) * TWO_PI / period) + 1);
        }
        [BurstCompile]
        public static float InOutElastic(float time, float duration, float overshootOrAmplitude, float period)
        {
            float s;
            if (time == 0) return 0;
            if ((time /= duration * 0.5f) == 2) return 1;
            if (period == 0) period = duration * (0.3f * 1.5f);
            if (overshootOrAmplitude < 1)
            {
                overshootOrAmplitude = 1;
                s = period / 4;
            }
            else s = period / TWO_PI * (float)Math.Asin(1 / overshootOrAmplitude);
            if (time < 1) return -0.5f * (overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * duration - s) * TWO_PI / period));
            return overshootOrAmplitude * (float)Math.Pow(2, -10 * (time -= 1)) * (float)Math.Sin((time * duration - s) * TWO_PI / period) * 0.5f + 1;
        }
        [BurstCompile]
        public static float InBack(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (time /= duration) * time * ((overshootOrAmplitude + 1) * time - overshootOrAmplitude);
        }
        [BurstCompile]
        public static float OutBack(float time, float duration, float overshootOrAmplitude, float period)
        {
            return ((time = time / duration - 1) * time * ((overshootOrAmplitude + 1) * time + overshootOrAmplitude) + 1);
        }
        [BurstCompile]
        public static float InOutBack(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration * 0.5f) < 1) return 0.5f * (time * time * (((overshootOrAmplitude *= (1.525f)) + 1) * time - overshootOrAmplitude));
            return 0.5f * ((time -= 2) * time * (((overshootOrAmplitude *= (1.525f)) + 1) * time + overshootOrAmplitude) + 2);
        }
        [BurstCompile]
        public static float InBounce(float time, float duration, float overshootOrAmplitude, float period)
        {
            return 1 - OutBounce(time, duration, overshootOrAmplitude, period);
        }
        [BurstCompile]
        public static float OutBounce(float time, float duration, float overshootOrAmplitude, float period)
        {
            if ((time /= duration) < (1 / 2.75f))
            {
                return (7.5625f * time * time);
            }
            if (time < (2 / 2.75f))
            {
                return (7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f);
            }
            if (time < (2.5f / 2.75f))
            {
                return (7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f);
            }
            return (7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f);
        }
        [BurstCompile]
        public static float InOutBounce(float time, float duration, float overshootOrAmplitude, float period)
        {
            if (time < duration * 0.5f)
            {
                return InBounce(time * 2, duration, -1, -1) * 0.5f;
            }
            return OutBounce(time * 2 - duration, duration, -1, -1) * 0.5f + 0.5f;
        }
    }
    #endregion

}
