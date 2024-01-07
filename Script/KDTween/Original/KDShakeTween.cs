using System;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin.Options;
using KD.Tweening.Plugin;

namespace KD.Tweening
{
    public class KDShakeTween<TValue, TPlugOptions> : KDTweenBase<TValue, TPlugOptions> where TPlugOptions : struct, IPlugOptions
    {
        private float m_ElapsedTime;

        private bool m_IsStart;

        private float m_Strength;       //揺れの強さ
        private float m_Vibrato = 10;   //振動
        private Vector3 m_RandomOffset; //ランダムのシード値

        internal override void Init()
        {
            base.Init();
            m_ElapsedTime = 0;
            m_IsStart = true;
        }

        #region Setter

        public void SetUp(float duration,KDGetter<TValue> getter,KDSetter<TValue> setter, float strength, float vibrato,Vector3 randomOffset)
        {
            Duration = duration;
            Getter = getter;
            Setter = setter;
            StartValue = getter();
            m_Strength = strength;
            m_Vibrato = vibrato;
            m_RandomOffset = randomOffset;
        }

        public void SetOption(TPlugOptions options)
        {
            m_Options = options;
        }

        #endregion


        #region Getter

        public float GetDelay() => Delay;
        public float GetDuration() => Duration;
        public Ease GetEase() => Easing;
        public UpdateType GetUpdateType() => UpdateType;
        public LoopType GetLoopType() => LoopType;
        public int GetLoopCount() => LoopCount;

        public float GetStrength() => m_Strength;
        public float GetVibrato() => m_Vibrato;

        public Vector3 GetRandomOffset() => m_RandomOffset;

        #endregion

        internal override void Update(float deltaTime)
        {
            //シークエンスがまだ開始されていない場合は待機させる
            if (IsSequence && !SequenceParent.IsPlay) return;

            //ポーズ中なら処理しない
            if (IsPause || IsSequence && SequenceParent.IsPause) return;

            if (IsUnscaled)
                deltaTime = Time.unscaledDeltaTime;


            if (Delay > 0)
            {
                Delay -= deltaTime;
                return;
            }

            m_ElapsedTime += deltaTime;

            ShakeTween();
        }

        private void ShakeTween()
        {
            //リンクしたオブジェクトが破棄された場合、一緒にTweenも破棄する
            if (IsLink && LinkObject == null)
            {
                Kill();
                return;
            }

            if (m_IsStart)
            {
                m_IsStart = false;

                //初期値が最初と違う場合の更新
                if (!StartValue.Equals(Getter()))
                    StartValueUpdate();

                //Start時に呼ぶコールバック
                OnTweenCallBack(onStart);
            }

            //処理時間を超えたらこのTweenを停止する
            if (m_ElapsedTime >= Duration)
            {
                m_ElapsedTime = Duration;
                IsComplete = true;
            }

            //Update中ずっと呼ぶコールバック
            OnTweenCallBack(onUpdate);

            //Tweenを実行する
            m_Plugin.EvaluateAndApply(this, m_Options, IsRelative, Getter, Setter, m_ElapsedTime, StartValue, EndValue, Duration);

            //終了時の処理
            if (IsComplete)
            {
                TweenComplete();

                //コールバックが存在しているなら処理する
                OnTweenCallBack(onComplete);
            }
        }

        /// <summary>
        /// Tween処理完了したら、Loopの処理を行うのかどうか判定する
        /// </summary>
        private void TweenComplete()
        {
            //元の値に戻す
            Setter(StartValue);
            Debugger.Log(StartValue);

            //ループしない場合はすぐにプールから削除する
            if (LoopCount == 0)
            {
                KDTweenManager.Instance.RemoveTween(this);
                return;
            }

            
            LoopCount--;
            IsComplete = false;
            m_ElapsedTime = 0;
            LoopSettings();
        }

        private void LoopSettings()
        {
            switch (LoopType)
            {
                case LoopType.Yoyo: //反復する

                    //値を反転する
                    TValue valueYoyo = EndValue;
                    EndValue = StartValue;
                    StartValue = valueYoyo;
                    break;
                case LoopType.Incremental: //現在の位置から追加で行う

                    TValue valueIncremental = EndValue;
                    EndValue = m_Plugin.SetRelativeEndValue(StartValue, EndValue);
                    StartValue = valueIncremental;
                    break;
                case LoopType.Restart: //最初の位置からもう一度行う

                    break;
            }
        }

        /// <summary>
        /// このTweenを破棄する
        /// </summary>
        public void Kill()
        {
            KDTweenManager.Instance.RemoveTween(this);

            //破棄されたときに呼びだすコールバック
            OnTweenCallBack(onKill);
        }

        #region CallBack
        public bool OnTweenCallBack(KDTweenCallback callback)
        {
            if (callback == null) return false;

            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// シークエンスやDoPathを作成するときに使用する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool OnTweenCallBack<T>(KDTweenCallback<T> callback, T param)
        {
            try
            {
                callback(param);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }
        #endregion
    }
}
