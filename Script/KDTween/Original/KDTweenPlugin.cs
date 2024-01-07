using System;
using UnityEngine;
using KD.Tweening.Core;
using KD.Tweening.Plugin.Options;
using KD.Tweening.Plugin;


namespace KD.Tweening.Plugin
{
    public interface KDTweenPlugin<TValue, TPlugOptions> : ITweenPlugin where TPlugOptions : struct, IPlugOptions
    {
        /// <summary>
        /// 初期化の処理
        /// </summary>
        /// <param name="t">Tween</param>
        public void Init(KDTweenBase<TValue, TPlugOptions> t);


        /// <summary>
        /// 変動値を設定する
        /// </summary>
        /// <param name="t">Tween</param>
        /// <param name="startValue">初期値</param>
        /// <param name="endValue">終了値</param>
        public void SetChangeValue(KDTweenBase<TValue,TPlugOptions> t,TValue startValue, TValue endValue);


        /// <summary>
        /// 相対的な値の場合の終了値の設定
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <returns></returns>
        public TValue SetRelativeEndValue(TValue startValue, TValue endValue);

        /// <summary>
        /// Tweenを実際に行う
        /// </summary>
        /// <param name="t">Tween</param>
        /// <param name="options">PlugOption</param>
        /// <param name="isRelative">相対的</param>
        /// <param name="getter">Tween中の現在地</param>
        /// <param name="setter">次のTweenの値として代入する値</param>
        /// <param name="elapsed">時間経過</param>
        /// <param name="startValue">初期値</param>
        /// <param name="endValue">終了値</param>
        /// <param name="duration">全体でかかる時間</param>
        public void EvaluateAndApply(KDTweenBase<TValue, TPlugOptions> t, TPlugOptions options, bool isRelative, KDGetter<TValue> getter, KDSetter<TValue> setter, float elapsed, TValue startValue, TValue endValue, float duration);
    }
}