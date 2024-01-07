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

        private float m_Strength;       //�h��̋���
        private float m_Vibrato = 10;   //�U��
        private Vector3 m_RandomOffset; //�����_���̃V�[�h�l

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
            //�V�[�N�G���X���܂��J�n����Ă��Ȃ��ꍇ�͑ҋ@������
            if (IsSequence && !SequenceParent.IsPlay) return;

            //�|�[�Y���Ȃ珈�����Ȃ�
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
            //�����N�����I�u�W�F�N�g���j�����ꂽ�ꍇ�A�ꏏ��Tween���j������
            if (IsLink && LinkObject == null)
            {
                Kill();
                return;
            }

            if (m_IsStart)
            {
                m_IsStart = false;

                //�����l���ŏ��ƈႤ�ꍇ�̍X�V
                if (!StartValue.Equals(Getter()))
                    StartValueUpdate();

                //Start���ɌĂԃR�[���o�b�N
                OnTweenCallBack(onStart);
            }

            //�������Ԃ𒴂����炱��Tween���~����
            if (m_ElapsedTime >= Duration)
            {
                m_ElapsedTime = Duration;
                IsComplete = true;
            }

            //Update�������ƌĂԃR�[���o�b�N
            OnTweenCallBack(onUpdate);

            //Tween�����s����
            m_Plugin.EvaluateAndApply(this, m_Options, IsRelative, Getter, Setter, m_ElapsedTime, StartValue, EndValue, Duration);

            //�I�����̏���
            if (IsComplete)
            {
                TweenComplete();

                //�R�[���o�b�N�����݂��Ă���Ȃ珈������
                OnTweenCallBack(onComplete);
            }
        }

        /// <summary>
        /// Tween��������������ALoop�̏������s���̂��ǂ������肷��
        /// </summary>
        private void TweenComplete()
        {
            //���̒l�ɖ߂�
            Setter(StartValue);
            Debugger.Log(StartValue);

            //���[�v���Ȃ��ꍇ�͂����Ƀv�[������폜����
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
                case LoopType.Yoyo: //��������

                    //�l�𔽓]����
                    TValue valueYoyo = EndValue;
                    EndValue = StartValue;
                    StartValue = valueYoyo;
                    break;
                case LoopType.Incremental: //���݂̈ʒu����ǉ��ōs��

                    TValue valueIncremental = EndValue;
                    EndValue = m_Plugin.SetRelativeEndValue(StartValue, EndValue);
                    StartValue = valueIncremental;
                    break;
                case LoopType.Restart: //�ŏ��̈ʒu���������x�s��

                    break;
            }
        }

        /// <summary>
        /// ����Tween��j������
        /// </summary>
        public void Kill()
        {
            KDTweenManager.Instance.RemoveTween(this);

            //�j�����ꂽ�Ƃ��ɌĂт����R�[���o�b�N
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
        /// �V�[�N�G���X��DoPath���쐬����Ƃ��Ɏg�p����
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