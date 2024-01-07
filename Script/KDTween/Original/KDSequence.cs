using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin;

namespace KD.Tweening
{
    public class KDSequence
    {
        internal float AppendIntervalTime;
        internal bool IsPlay;
        internal bool IsPause;

        //インスタンス
        public KDSequence()
        {
            AppendIntervalTime = 0;

            IsPlay = false;
            IsPause = false;
        }

        public KDSequence Append(IKDTween t)
        {
            if (t.LoopCount < 0)
            {
                Debug.LogError("無限ループをsequenceで使用しないでください");
                return this;
            }

            AppendIntervalTime += t.Delay + (t.Duration * (t.LoopCount + 1));


            //Tweenの設定
            t.SequenceParent = this;
            t.IsSequence = true;
            t.Delay = AppendIntervalTime - t.Duration;

            return this;
        }

        public KDSequence Insert(float delay, IKDTween t)
        {
            AppendIntervalTime += delay;

            //Tweenの設定
            t.SequenceParent = this;
            t.IsSequence = true;
            t.Delay += AppendIntervalTime - t.Duration;

            return this;
        }

        //待ち時間を追加する
        public KDSequence AppendInterval(float delay)
        {
            AppendIntervalTime += delay;
            return this;
        }

        /// <summary>
        /// sequenceを実行する
        /// </summary>
        public void Play()
        {
            if (IsPlay) return;

            IsPlay = true;
            return;
        }

        /// <summary>
        /// sequenceを再開する
        /// </summary>
        public KDSequence ReStart()
        {
            IsPause = false;

            return this;
        }

        /// <summary>
        /// sequenceを中断する
        /// </summary>
        public KDSequence Pause()
        {
            IsPause = true;

            return this;
        }
    }
}