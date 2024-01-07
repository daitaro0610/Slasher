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
        /// �������̏���
        /// </summary>
        /// <param name="t">Tween</param>
        public void Init(KDTweenBase<TValue, TPlugOptions> t);


        /// <summary>
        /// �ϓ��l��ݒ肷��
        /// </summary>
        /// <param name="t">Tween</param>
        /// <param name="startValue">�����l</param>
        /// <param name="endValue">�I���l</param>
        public void SetChangeValue(KDTweenBase<TValue,TPlugOptions> t,TValue startValue, TValue endValue);


        /// <summary>
        /// ���ΓI�Ȓl�̏ꍇ�̏I���l�̐ݒ�
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <returns></returns>
        public TValue SetRelativeEndValue(TValue startValue, TValue endValue);

        /// <summary>
        /// Tween�����ۂɍs��
        /// </summary>
        /// <param name="t">Tween</param>
        /// <param name="options">PlugOption</param>
        /// <param name="isRelative">���ΓI</param>
        /// <param name="getter">Tween���̌��ݒn</param>
        /// <param name="setter">����Tween�̒l�Ƃ��đ������l</param>
        /// <param name="elapsed">���Ԍo��</param>
        /// <param name="startValue">�����l</param>
        /// <param name="endValue">�I���l</param>
        /// <param name="duration">�S�̂ł����鎞��</param>
        public void EvaluateAndApply(KDTweenBase<TValue, TPlugOptions> t, TPlugOptions options, bool isRelative, KDGetter<TValue> getter, KDSetter<TValue> setter, float elapsed, TValue startValue, TValue endValue, float duration);
    }
}