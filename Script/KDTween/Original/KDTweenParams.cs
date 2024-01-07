using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KD.Tweening.Plugin;

namespace KD.Tweening
{
    public static class KDTweenParams
    {
        public static KDTweenBase<TValue, TPlugOptions> OnPlay<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, KDTweenCallback action)
            where TPlugOptions : struct, IPlugOptions
        {
            t.onPlay = action;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> OnStart<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, KDTweenCallback action)
            where TPlugOptions : struct, IPlugOptions
        {
            t.onStart = action;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> OnUpdate<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, KDTweenCallback action)
            where TPlugOptions : struct, IPlugOptions
        {
            t.onUpdate = action;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> OnStepComplete<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, KDTweenCallback action)
             where TPlugOptions : struct, IPlugOptions
        {
            t.onStepComplete = action;
            return t;
        }
        public static KDTweenBase<TValue, TPlugOptions> OnComplete<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, KDTweenCallback action)
            where TPlugOptions : struct, IPlugOptions
        {
            t.onComplete = action;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> OnKill<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, KDTweenCallback action)
            where TPlugOptions : struct, IPlugOptions
        {
            t.onKill = action;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetRelative<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t)
            where TPlugOptions : struct, IPlugOptions
        {
            t.IsRelative = true;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetEase<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, Ease ease)
            where TPlugOptions : struct, IPlugOptions
        {
            t.Easing = ease;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetDelay<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, float delay)
            where TPlugOptions : struct, IPlugOptions
        {
            t.Delay = delay;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetLoops<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, int loops,LoopType loopType)
            where TPlugOptions : struct, IPlugOptions
        {
            t.LoopCount = loops;
            t.LoopType = loopType;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetLink<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, GameObject gameObject)
            where TPlugOptions : struct, IPlugOptions
        {
            t.IsLink = true;
            t.LinkObject = gameObject;
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetUpdateType<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t, UpdateType updateType)
            where TPlugOptions : struct, IPlugOptions
        {
            t.UpdateType = updateType;
            KDTweenManager.Instance.ChangeTweenUpdate(t);
            return t;
        }

        public static KDTweenBase<TValue, TPlugOptions> SetUnscaled<TValue, TPlugOptions>(this KDTweenBase<TValue, TPlugOptions> t)
            where TPlugOptions : struct, IPlugOptions
        {
            t.IsUnscaled = true;
            return t;
        }
    }
}