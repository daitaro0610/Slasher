using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KD
{
    public class FadeManager : MonoBehaviour
    {
        public static FadeManager instance;
        [SerializeField]
        private Image m_FadeImg;//�t�F�[�h�����邽�߂�

        private float m_FadeInTime;
        private float m_FadeOutTime;
        private float m_FadeTime;

        private float m_SetAlphaValue;
        private float m_EndAlphaValue;

        [SerializeField]
        GameObject m_FadeCanvas;

        GameObject m_CurrentFadeObj;

        private bool IsUnscaled;
        private bool IsDefaultScaled = false;

        private float m_deltaTime;

        //�������I��������ɃR�[���o�b�N����̂Ɏg��
        public delegate void OnCompleteDelegate();
        private OnCompleteDelegate OnComplete;

        /// <summary>
        /// �������I��������ɌĂяo��
        /// </summary>
        /// <param name="onCompleteDelegate">�Ăт�������</param>
        public void OnCompleted(OnCompleteDelegate onCompleteDelegate) => OnComplete = onCompleteDelegate;

        public delegate void OnFadeInFadeOutComplete();
        private OnFadeInFadeOutComplete OnFadeInOutComplete;

        /// <summary>
        /// FadeOut���I������^�C�~���O�ŌĂт���
        /// </summary>
        /// <param name="onFadeInFadeOutComplete">�Ăт�������</param>
        /// <returns></returns>
        public FadeManager OnFadeInOutCompleted(OnFadeInFadeOutComplete onFadeInFadeOutComplete)
        {
            OnFadeInOutComplete = onFadeInFadeOutComplete;
            return this;
        }
        /// <summary>
        /// Fade�̏�Ԃ��Ǘ�����State
        /// </summary>
        private enum State
        {
            Normal,
            Fade,
            FadeIn,
            FadeOut,
            FadeInOut,
        }
        State m_State = State.Normal;

        private void Awake()
        {
            //�V���O���g���@�V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                GameObject fadeCanvas = Instantiate(m_FadeCanvas);
                DontDestroyOnLoad(fadeCanvas);
                m_FadeImg = fadeCanvas.GetComponentInChildren<Image>();
                IsDefaultScaled = false;
                IsUnscaled = IsDefaultScaled;

                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// �V�[�����ǂݍ��܂ꂽ�Ƃ��ɌĂ΂��
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (m_FadeImg == null)
            {
                m_FadeImg = GameObject.FindGameObjectWithTag("FadeImage").GetComponent<Image>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (IsUnscaled) m_deltaTime = Time.unscaledDeltaTime;
            else m_deltaTime = Time.deltaTime;

            switch (m_State)
            {
                case State.Fade: FadeUpdate(); break;
                case State.FadeIn: FadeInUpdate(); break;
                case State.FadeOut: FadeOutUpdate(); break;
                case State.FadeInOut: FadeInOutUpdate(); break;
                default: break;
            }
        }

        public FadeManager ChangeColor(Color color)
        {
            //�����x�̓L�[�v
            color = new Color(color.r, color.g, color.b, m_FadeImg.color.a);
            m_FadeImg.color = color;
            return this;
        }
        private float m_FadeAlphaValue;

        /// <summary>
        /// ���̒l������̒l�܂ŕω�������
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startAlpha"></param>
        /// <param name="endAlpha"></param>
        /// <returns></returns>
        public FadeManager Fade(float time, float startAlpha, float endAlpha, bool isUnscaled = false)
        {
            m_FadeTime = time;
            m_State = State.Fade;
            m_EndAlphaValue = endAlpha;
            IsUnscaled = isUnscaled;

            //�l�𑝂₷�̂����炷�̂��𔻒�
            m_FadeAlphaValue = startAlpha - endAlpha;
            //�����x�̏����l��ݒ�
            FadeObjAlphaSetUp(startAlpha);
            return this;
        }

        private void FadeUpdate()
        {
            if (m_FadeImg.color.a <= m_EndAlphaValue)
            {
                if (m_FadeAlphaValue < 0)
                    m_FadeImg.color -= new Color(0, 0, 0, m_FadeAlphaValue * m_deltaTime / m_FadeTime);
            }
            else if (m_FadeImg.color.a >= m_EndAlphaValue)
            {
                if (m_FadeAlphaValue > 0)
                    m_FadeImg.color -= new Color(0, 0, 0, m_FadeAlphaValue * m_deltaTime / m_FadeTime);
            }
            else
            {
                m_State = State.Normal;
                m_FadeImg.color = new Color(m_FadeImg.color.r, m_FadeImg.color.g, m_FadeImg.color.b, m_EndAlphaValue);
                //�R�[���o�b�N���Ă�
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                    OnComplete = null;
                    IsUnscaled = IsDefaultScaled;
                }
            }
        }

        /// <summary>
        /// ���X�ɉ�ʂ𖾂邭����
        /// </summary>
        /// <param name="time">�����鎞��</param>
        public FadeManager FadeIn(float time, bool isUnscaled = false)
        {
            m_FadeInTime = time;
            IsUnscaled = isUnscaled;
            FadeObjAlphaSetUp(1.0f);
            m_State = State.FadeIn;
            return this;
        }

        /// <summary>
        /// �t�F�[�h�C������Update�ŌĂ΂��֐�
        /// </summary>
        private void FadeInUpdate()
        {
            if (m_FadeImg.color.a >= 0)
                m_FadeImg.color -= new Color(0, 0, 0, m_deltaTime / m_FadeInTime);
            else
            {
                m_State = State.Normal;
                m_FadeImg.color = new Color(m_FadeImg.color.r, m_FadeImg.color.g, m_FadeImg.color.b, 0);
                //�R�[���o�b�N���Ă�
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                    OnComplete = null;
                    IsUnscaled = IsDefaultScaled;
                }
            }
        }

        /// <summary>
        /// ���X�ɉ�ʂ��Â�����
        /// </summary>
        /// <param name="time">�����鎞��</param>
        public FadeManager FadeOut(float time, bool isUnscaled = false)
        {
            m_FadeOutTime = time;
            IsUnscaled = isUnscaled;
            FadeObjAlphaSetUp(0.0f);
            m_State = State.FadeOut;
            return this;
        }

        /// <summary>
        /// �t�F�[�h�A�E�g����Update�ŌĂ΂��֐�
        /// </summary>
        private void FadeOutUpdate()
        {
            if (m_FadeImg.color.a <= 1)
                m_FadeImg.color += new Color(0, 0, 0, m_deltaTime / m_FadeOutTime);
            else
            {
                m_FadeImg.color = new Color(m_FadeImg.color.r, m_FadeImg.color.g, m_FadeImg.color.b, 1);
                m_State = State.Normal;
                //�R�[���o�b�N���Ă�
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                    OnComplete = null;
                    IsUnscaled = IsDefaultScaled;
                }
            }
        }

        /// <summary>
        /// �Â��Ȃ�����ɂ܂����邭�Ȃ�
        /// </summary>
        /// <param name="fadeOutTime">�t�F�[�h�A�E�g�̎���</param>
        /// <param name="fadeInTime">�t�F�[�h�C���̎���</param>
        public FadeManager FadeInOut(float fadeOutTime, float fadeInTime, bool isUnscaled = false)
        {
            m_FadeOutTime = fadeOutTime;
            IsUnscaled = isUnscaled;
            m_FadeInTime = fadeInTime;
            FadeObjAlphaSetUp(0.0f);
            m_State = State.FadeInOut;
            return this;
        }

        /// <summary>
        /// �t�F�[�h�A�E�g������Ƀt�F�[�h�C������Update�ŌĂ΂��֐�
        /// </summary>
        private void FadeInOutUpdate()
        {
            if (m_FadeImg.color.a <= 1)
                m_FadeImg.color += new Color(0, 0, 0, m_deltaTime / m_FadeOutTime);
            else
            {
                //�t�F�[�h�A�E�g���I�������A�X�e�[�g���t�F�[�h�C���ɂ���
                m_FadeImg.color = new Color(m_FadeImg.color.r, m_FadeImg.color.g, m_FadeImg.color.b, 1);
                m_State = State.FadeIn;

                //�t�F�[�h�C���t�F�[�h�A�E�g�̒��Ԃł�����Ăяo��
                if(OnFadeInOutComplete != null)
                {
                    OnFadeInOutComplete.Invoke();
                    OnFadeInOutComplete = null;
                }
            }
        }
        /// <summary>
        /// �t�F�[�h�I�u�W�F�N�g�̃A���t�@�̐ݒ�
        /// </summary>
        /// <param name="alpha">�������̃A���t�@�̒l</param>
        public void FadeObjAlphaSetUp(float alpha)
        {
            //�l��1���傫���A0������������ő�A�ŏ��l�ɒ���
            if (alpha > 1) alpha = 1;
            else if (alpha < 0) alpha = 0;

            m_SetAlphaValue = alpha;

            //�I�u�W�F�N�g�̐���
            //�����x��Fade�ɉ����ĕύX����
            m_FadeImg.color = new Color(m_FadeImg.color.r, m_FadeImg.color.g, m_FadeImg.color.b, m_SetAlphaValue);
        }

        public FadeManager SetUnScaledDeltaTime(bool isbool) {
            IsDefaultScaled = isbool;
            return this;
        } 
    }
}
