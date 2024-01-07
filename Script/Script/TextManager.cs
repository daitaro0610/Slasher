using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace KD
{
    public class TextManager : MonoBehaviour
    {
        [SerializeField]
        TextDataBase m_TextDataBase;
        TextDataBase.TextData m_TextData;

        public static TextManager instance;

        [SerializeField]
        private float m_DialogueWaitTime = 0.2f;

        private float m_TextTime;

        [HideInInspector]
        public int m_DataArray = 0;
        private int m_TextAttay = 0;

        private int m_NextTextCount;
        private bool is_NextText;

        [HideInInspector]
        public bool is_CompleteText = true;

        [SerializeField,Header("�X�L�b�v�@�\���������邩�ǂ���")]
        private bool is_SkipText;

        private string[] m_Words;

        WaitForSeconds m_WaitTextTime;

        public delegate void OnCompleteDelegate();
        private OnCompleteDelegate OnComplete;
        public void OnCompleted(OnCompleteDelegate onCompleteDelegate) => OnComplete = onCompleteDelegate;

        private void Awake()
        {
            //�V���O���g�����C���X�^���X������
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            m_WaitTextTime = new WaitForSeconds(m_DialogueWaitTime);
        }

        private void Update()
        {
            //�e�L�X�g��\�����Ă���r���̎��A�������������當������C�ɕ\������
            if (Input.anyKeyDown && !is_CompleteText && is_SkipText)
                m_TextTime = 0;
        }

        /// <summary>
        /// TextData�̂ǂ̔ԍ���ǂݍ��ނ��擾����
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public TextManager ChangeTextDataArray(int array)
        {
            if (array >= m_TextDataBase.m_TextData.Length)
                Debug.LogError("�z��𒴂��Ă��܂� Length:" + m_TextDataBase.m_TextData.Length +" num:"+array);
            m_DataArray = array;
            return this;
        }

        /// <summary>
        /// ������ǂݍ���
        /// </summary>
        /// <param name="text">�e�L�X�g�R���|�[�l���g</param>
        /// <param name="words">����</param>
        /// <param name="readSpeed">������\������X�s�[�h</param>
        /// <returns></returns>
        public TextManager LoadText(Text text, string words, float readSpeed)
        {
            if (!is_CompleteText) return this;

            m_TextTime = readSpeed;

            string insertWord = InsertWords(words);

            StartCoroutine(Dialogue(insertWord, text));

            return this;
        }

        /// <summary>
        /// ���������X�ɕ\������
        /// </summary>
        /// <param name="text">�e�L�X�g�R���|�[�l���g</param>
        /// <param name="index">���Ԗڂ̕������\�����邩</param>
        /// <returns></returns>
        public TextManager ReadText(Text text, int index = -1)
        {
            if (!is_CompleteText) return this;

            //���ڈȍ~�͏��ԂɌĂяo��
            if (index >= 0) m_TextAttay = index;
            else m_TextAttay++;

            if (m_TextDataBase.m_TextData[m_DataArray].Texts.Length <= m_TextAttay)
                Debug.LogError("�z�񒴉߂��Ă��܂� :" + m_TextAttay);

            m_TextTime = m_TextDataBase.m_TextData[m_DataArray].TextSpeed;
            is_CompleteText = false;

            string insertWord = InsertWords(m_TextDataBase.m_TextData[m_DataArray].Texts[m_TextAttay]);

            StartCoroutine(Dialogue(insertWord, text));

            return this;
        }

        //�ǂ̂��炢�̕�����\���������邩
        public TextManager NextTextCount(int textCount)
        {
            is_NextText = true;
            m_NextTextCount = textCount;
            return this;
        }

        /// <summary>
        /// ���̕������Ăяo��
        /// </summary>
        /// <returns></returns>
        public TextManager Next(Text text)
        {
            ReadText(text);
            return this;
        }

        /// <summary>
        /// �������ꕶ�����\������
        /// </summary>
        /// <returns></returns>
        private IEnumerator Dialogue(string words, Text text)
        {
            //���p�X�y�[�X�ŋ�؂�
            m_Words = words.Split(' ');
            text.text = "";
            yield return m_WaitTextTime;
            yield return null;
            //�������w�莞�Ԃ��\�����Ă���
            foreach (var word in m_Words)
            {
                text.text += word;
                yield return new WaitForSeconds(m_TextTime);
            }

            //�������I�������True�ɂ���
            is_CompleteText = true;


            //�������J�E���g�̉񐔕�����ׂ点��
            if (is_NextText)
            {
                m_NextTextCount--;
                if (m_NextTextCount > 0)
                {
                    yield return new WaitForSeconds(1.0f);
                    ReadText(text);
                }
                else is_NextText = false;
            }
            else if (OnComplete != null)
            {
                OnComplete.Invoke();
                OnComplete = null;
            }
        }

        private string InsertWords(string word)
        {
            int wordsCount = word.Length;

            //���p�X�y�[�X�ŋ�؂邽�߂Ɉꕶ�����Ƃɔ��p�X�y�[�X��ǉ�����
            for (int i = 0; i < (wordsCount * 2) - 1; i++)
            {
                //��̎���������
                if (i % 2 == 1)
                {
                    string talk = word.Insert(i, " ");
                    word = talk;
                }
            }

            return word;
        }
    }
}