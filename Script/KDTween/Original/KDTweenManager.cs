using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin;
using Unity.Burst;

namespace KD.Tweening
{
    [DefaultExecutionOrder(-100)]
    public class KDTweenManager : MonoBehaviour
    {
        private KDTweenPool m_TweenPool;
        public static KDTweenManager Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if(Instance == null)
            {
                GameObject tweenManagerObj = new GameObject("TweenManager");
                tweenManagerObj.AddComponent<KDTweenManager>();
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                m_TweenPool = new KDTweenPool();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            m_TweenPool.Update();
        }

        private void LateUpdate()
        {
            m_TweenPool.LateUpdate();
        }

        private void FixedUpdate()
        {
            m_TweenPool.FixedUpdate();
        }

        public KDTween<TValue, TPlugOptions> SetUp<TValue, TPlugOptions>(KDGetter<TValue> getter, KDSetter<TValue> setter, TValue endValue, float duration, TPlugOptions options)
        where TPlugOptions : struct, IPlugOptions
        {
            KDTween<TValue, TPlugOptions> t = m_TweenPool.GetNonActiveTween<TValue, TPlugOptions>();

            if (t == null)
            {
                Debug.LogError("TweenëŒè€Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩ");
                return null;
            }

            t.Init();
            t.SetUp(duration, getter, setter, endValue);
            t.SetOptions(options);
            return t;
        }

        public KDShakeTween<TValue, TPlugOptions> SetUp<TValue, TPlugOptions>(KDGetter<TValue> getter, KDSetter<TValue> setter, float strength, float vibrato, Vector3 randomOffset, float duration, TPlugOptions options)
        where TPlugOptions : struct, IPlugOptions
        {
            KDShakeTween<TValue, TPlugOptions> t = m_TweenPool.GetNonActiveShakeTween<TValue, TPlugOptions>();

            if (t == null)
            {
                Debug.LogError("ShakeëŒè€Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩ");
                return null;
            }

            t.Init();
            t.SetUp(duration, getter, setter, strength, vibrato, randomOffset);
            t.SetOption(options);
            return t;
        }

        public void AddTween(IKDTween t)
        {
            m_TweenPool.AddTween(t);
        }

        public void RemoveTween(IKDTween t)
        {
            m_TweenPool.RemoveTween(t);
        }

        public void ChangeTweenUpdate(IKDTween t)
        {
            m_TweenPool.ChangeTweenUpdate(t);
        }
    }
}