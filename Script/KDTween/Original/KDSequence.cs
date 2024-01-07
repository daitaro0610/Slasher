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

        //�C���X�^���X
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
                Debug.LogError("�������[�v��sequence�Ŏg�p���Ȃ��ł�������");
                return this;
            }

            AppendIntervalTime += t.Delay + (t.Duration * (t.LoopCount + 1));


            //Tween�̐ݒ�
            t.SequenceParent = this;
            t.IsSequence = true;
            t.Delay = AppendIntervalTime - t.Duration;

            return this;
        }

        public KDSequence Insert(float delay, IKDTween t)
        {
            AppendIntervalTime += delay;

            //Tween�̐ݒ�
            t.SequenceParent = this;
            t.IsSequence = true;
            t.Delay += AppendIntervalTime - t.Duration;

            return this;
        }

        //�҂����Ԃ�ǉ�����
        public KDSequence AppendInterval(float delay)
        {
            AppendIntervalTime += delay;
            return this;
        }

        /// <summary>
        /// sequence�����s����
        /// </summary>
        public void Play()
        {
            if (IsPlay) return;

            IsPlay = true;
            return;
        }

        /// <summary>
        /// sequence���ĊJ����
        /// </summary>
        public KDSequence ReStart()
        {
            IsPause = false;

            return this;
        }

        /// <summary>
        /// sequence�𒆒f����
        /// </summary>
        public KDSequence Pause()
        {
            IsPause = true;

            return this;
        }
    }
}