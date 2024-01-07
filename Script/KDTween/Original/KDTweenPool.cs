using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening
{
    public class KDTweenPool
    {
        private readonly List<IKDTween> NonActiveTween = new();

        private readonly List<IKDTween> UpdateActiveTween = new();
        private readonly List<IKDTween> LateUpdateActiveTween = new();
        private readonly List<IKDTween> FixedUpdateActiveTween = new();

        public KDTweenPool()
        {
            //初期化
            NonActiveTween = new();
            UpdateActiveTween = new();
            LateUpdateActiveTween = new();
            FixedUpdateActiveTween = new();
        }

        /// <summary>
        /// 非アクティブなTweenを検索して取り出す
        /// </summary>
        /// <typeparam name="TValue">指定された型</typeparam>
        /// <typeparam name="TPlugOptions">その型のオプション</typeparam>
        /// <returns></returns>
        public KDTween<TValue, TPlugOptions> GetNonActiveTween<TValue, TPlugOptions>()
            where TPlugOptions : struct, IPlugOptions
        {
            //非アクティブなTweenを検索してそれを使用する　
            //指定した型のTweenがあるか探している
            for (int i = 0; i < NonActiveTween.Count; i++)
            {
                if (NonActiveTween[i] is KDTween<TValue, TPlugOptions> tween)
                {
                    AddTween(tween);
                    NonActiveTween.Remove(tween);
                    return tween;
                }
            }

            //見つからない場合は新しく作成する
            KDTween<TValue, TPlugOptions> t = new();
            AddTween(t);

            return t;
        }

        /// <summary>
        /// 非アクティブなTweenを検索して取り出す
        /// </summary>
        /// <typeparam name="TValue">指定された型</typeparam>
        /// <typeparam name="TPlugOptions">その型のオプション</typeparam>
        /// <returns></returns>
        public KDShakeTween<TValue, TPlugOptions> GetNonActiveShakeTween<TValue, TPlugOptions>()
            where TPlugOptions : struct, IPlugOptions
        {
            //非アクティブなTweenを検索してそれを使用する　
            //指定した型のTweenがあるか探している
            for (int i = 0; i < NonActiveTween.Count; i++)
            {
                if (NonActiveTween[i] is KDShakeTween<TValue, TPlugOptions> tween)
                {
                    AddTween(tween);
                    NonActiveTween.Remove(tween);
                    return tween;
                }
            }

            //見つからない場合は新しく作成する
            KDShakeTween<TValue, TPlugOptions> t = new();
            AddTween(t);

            return t;
        }

        public void Update()
        {
            for (int i = 0; i < UpdateActiveTween.Count; i++)
            {
                UpdateActiveTween[i].Update(Time.deltaTime);
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < LateUpdateActiveTween.Count; i++)
            {
                LateUpdateActiveTween[i].Update(Time.deltaTime);
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < FixedUpdateActiveTween.Count; i++)
            {
                FixedUpdateActiveTween[i].Update(Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Tweenを指定したUpdateに追加する
        /// </summary>
        /// <param name="t"></param>
        /// <param name="type"></param>
        public void AddTween(IKDTween t)
        {
            switch (t.UpdateType)
            {
                case UpdateType.Normal: UpdateActiveTween.Add(t); break;
                case UpdateType.Late: LateUpdateActiveTween.Add(t); break;
                case UpdateType.Fixed: FixedUpdateActiveTween.Add(t); break;
            }
        }

        /// <summary>
        /// TweenのUpdateTypeに応じて、削除するリストを変更する
        /// </summary>
        /// <param name="t"></param>
        /// <param name="type"></param>
        public void RemoveTween(IKDTween t)
        {
            switch (t.UpdateType)
            {
                case UpdateType.Normal: UpdateActiveTween.Remove(t); break;
                case UpdateType.Late: LateUpdateActiveTween.Remove(t); break;
                case UpdateType.Fixed: FixedUpdateActiveTween.Remove(t); break;
            }

            NonActiveTween.Add(t);
        }

        /// <summary>
        /// UpdateTypeが変更されたときに呼びだす
        /// </summary>
        /// <param name="t"></param>
        /// <param name="type"></param>
        public void ChangeTweenUpdate(IKDTween t)
        {
            switch (t.UpdateType)
            {
                case UpdateType.Normal:

                    if (LateUpdateActiveTween.Contains(t))
                    {
                        LateUpdateActiveTween.Remove(t);
                    }
                    else if (FixedUpdateActiveTween.Contains(t))
                    {
                        FixedUpdateActiveTween.Contains(t);
                    }

                    UpdateActiveTween.Add(t);

                    break;
                case UpdateType.Late:

                    if (UpdateActiveTween.Contains(t))
                    {
                        UpdateActiveTween.Remove(t);
                    }
                    else if (FixedUpdateActiveTween.Contains(t))
                    {
                        FixedUpdateActiveTween.Contains(t);
                    }

                    LateUpdateActiveTween.Add(t);

                    break;
                case UpdateType.Fixed:

                    if (UpdateActiveTween.Contains(t))
                    {
                        UpdateActiveTween.Remove(t);
                    }
                    else if (LateUpdateActiveTween.Contains(t))
                    {
                        LateUpdateActiveTween.Contains(t);
                    }

                    FixedUpdateActiveTween.Add(t);

                    break;
            }


        }
    }
}
