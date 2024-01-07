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

        [SerializeField,Header("スキップ機能を実装するかどうか")]
        private bool is_SkipText;

        private string[] m_Words;

        WaitForSeconds m_WaitTextTime;

        public delegate void OnCompleteDelegate();
        private OnCompleteDelegate OnComplete;
        public void OnCompleted(OnCompleteDelegate onCompleteDelegate) => OnComplete = onCompleteDelegate;

        private void Awake()
        {
            //シングルトンかつインスタンス化する
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
            //テキストを表示している途中の時、何かを押したら文字を一気に表示する
            if (Input.anyKeyDown && !is_CompleteText && is_SkipText)
                m_TextTime = 0;
        }

        /// <summary>
        /// TextDataのどの番号を読み込むか取得する
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public TextManager ChangeTextDataArray(int array)
        {
            if (array >= m_TextDataBase.m_TextData.Length)
                Debug.LogError("配列を超えています Length:" + m_TextDataBase.m_TextData.Length +" num:"+array);
            m_DataArray = array;
            return this;
        }

        /// <summary>
        /// 文字を読み込む
        /// </summary>
        /// <param name="text">テキストコンポーネント</param>
        /// <param name="words">文字</param>
        /// <param name="readSpeed">文字を表示するスピード</param>
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
        /// 文字を徐々に表示する
        /// </summary>
        /// <param name="text">テキストコンポーネント</param>
        /// <param name="index">何番目の文字列を表示するか</param>
        /// <returns></returns>
        public TextManager ReadText(Text text, int index = -1)
        {
            if (!is_CompleteText) return this;

            //二回目以降は順番に呼び出す
            if (index >= 0) m_TextAttay = index;
            else m_TextAttay++;

            if (m_TextDataBase.m_TextData[m_DataArray].Texts.Length <= m_TextAttay)
                Debug.LogError("配列超過しています :" + m_TextAttay);

            m_TextTime = m_TextDataBase.m_TextData[m_DataArray].TextSpeed;
            is_CompleteText = false;

            string insertWord = InsertWords(m_TextDataBase.m_TextData[m_DataArray].Texts[m_TextAttay]);

            StartCoroutine(Dialogue(insertWord, text));

            return this;
        }

        //どのくらいの文字を表示し続けるか
        public TextManager NextTextCount(int textCount)
        {
            is_NextText = true;
            m_NextTextCount = textCount;
            return this;
        }

        /// <summary>
        /// 次の文字を呼び出す
        /// </summary>
        /// <returns></returns>
        public TextManager Next(Text text)
        {
            ReadText(text);
            return this;
        }

        /// <summary>
        /// 文字を一文字ずつ表示する
        /// </summary>
        /// <returns></returns>
        private IEnumerator Dialogue(string words, Text text)
        {
            //半角スペースで区切る
            m_Words = words.Split(' ');
            text.text = "";
            yield return m_WaitTextTime;
            yield return null;
            //文字を指定時間ずつ表示していく
            foreach (var word in m_Words)
            {
                text.text += word;
                yield return new WaitForSeconds(m_TextTime);
            }

            //処理が終わったらTrueにする
            is_CompleteText = true;


            //文字をカウントの回数分しゃべらせる
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

            //半角スペースで区切るために一文字ごとに半角スペースを追加する
            for (int i = 0; i < (wordsCount * 2) - 1; i++)
            {
                //奇数の時だけ判定
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