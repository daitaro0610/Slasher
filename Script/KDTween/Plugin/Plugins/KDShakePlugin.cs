using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;

using Random = UnityEngine.Random;

namespace KD.Tweening.Plugin
{
    public struct KDShakePlugin : KDTweenPlugin<Vector3, ShakeOptions>
    {
        private KDShakeTween<Vector3, ShakeOptions> m_Tween;

        public void Init(KDTweenBase<Vector3, ShakeOptions> t)
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

        public void EvaluateAndApply(KDTweenBase<Vector3, ShakeOptions> t, ShakeOptions options, bool isRelative, KDGetter<Vector3> getter, KDSetter<Vector3> setter, float elapsed, Vector3 startValue, Vector3 endValue, float duration)
        {
            if (t is KDShakeTween<Vector3, ShakeOptions> tween)
            {
                EvaluateAndApply(tween, options, getter, setter, elapsed, tween.GetStrength(), tween.GetVibrato(),tween.GetRandomOffset(), duration);
            }
        }

        /// <summary>
        /// 振動させる
        /// </summary>
        public void EvaluateAndApply(KDShakeTween<Vector3, ShakeOptions> t, ShakeOptions options, KDGetter<Vector3> getter, KDSetter<Vector3> setter, float elapsed, float strength, float vibrato,Vector3 randomOffset, float duration)
        {
            float easeVal = EaseManager.Evaluate(t.Easing, elapsed, duration, t.OvershootOrAmplitude, t.Period);

            // 初期位置-vibrato ~ 初期位置+vibrato の間に収める
            var ratio = 1.0f - easeVal;
            if (options.fadeOut)
                vibrato *= ratio; // フェードアウトさせるため、経過時間により揺れの量を減衰

            //初期値に対して加える処理を行う
            var position = t.StartValue;

            switch (options.axisConstraint)
            {
                case AxisConstraint.X:
                    {
                        var randomX = GetPerlinNoiseValue(randomOffset.x, strength, elapsed);
                        randomX = Mathf.Lerp(-vibrato, vibrato, randomX);

                        position.x += randomX;
                        setter(position);
                    }
                    break;

                case AxisConstraint.Y:
                    {
                        var randomY = GetPerlinNoiseValue(randomOffset.y, strength, elapsed);
                        randomY = Mathf.Lerp(-vibrato, vibrato, randomY);

                        position.y += randomY;
                        setter(position);
                    }
                    break;

                case AxisConstraint.Z:
                    {
                        var randomZ = GetPerlinNoiseValue(randomOffset.z, strength, elapsed);
                        randomZ = Mathf.Lerp(-vibrato, vibrato, randomZ);

                        position.z += randomZ;
                        setter(position);
                    }
                    break;

                case AxisConstraint.XY:
                    {
                        var randomX = GetPerlinNoiseValue(randomOffset.x, strength, elapsed);
                        var randomY = GetPerlinNoiseValue(randomOffset.y, strength, elapsed);

                        randomX = Mathf.Lerp(-vibrato, vibrato, randomX);
                        randomY = Mathf.Lerp(-vibrato, vibrato, randomY);

                        position.x += randomX;
                        position.y += randomY;
                        setter(position);
                    }
                    break;
                default:
                    {
                        var randomX = GetPerlinNoiseValue(randomOffset.x, strength, elapsed);
                        var randomY = GetPerlinNoiseValue(randomOffset.y, strength, elapsed);
                        var randomZ = GetPerlinNoiseValue(randomOffset.z, strength, elapsed);

                        randomX = Mathf.Lerp(-vibrato, vibrato, randomX);
                        randomY = Mathf.Lerp(-vibrato, vibrato, randomY);
                        randomZ = Mathf.Lerp(-vibrato, vibrato, randomZ);

                        position.x += randomX;
                        position.y += randomY;
                        position.z += randomZ;
                        setter(position);
                    }
                    break;
            }

        }

        /// <summary>
        /// パーリンノイズ値を取得
        /// </summary>
        private float GetPerlinNoiseValue(float offset, float speed, float time)
        {
            // パーリンノイズ値を取得する
            // X: オフセット値 + 速度 * 時間
            // Y: 0.0固定
            return Mathf.PerlinNoise(offset + (speed * time), 0.0f);
        }

        public void SetChangeValue(KDTweenBase<Vector3, ShakeOptions> t, Vector3 startValue, Vector3 endValue)
        {
            t.ChangeValue = endValue - startValue;
        }

        public Vector3 SetRelativeEndValue(Vector3 startValue, Vector3 endValue)
        {
            return endValue + endValue - startValue;
        }
    }

}