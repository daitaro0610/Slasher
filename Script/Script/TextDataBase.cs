using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "CreateData/TextDataBase")]
public class TextDataBase : ScriptableObject
{
    public TextData[] m_TextData;

    [System.Serializable]
    public class TextData
    {
        [SerializeField, Tooltip("名前")]
        string m_Name;

        [SerializeField, Tooltip("テキスト")]
        [Multiline(3)]
        string[] m_Texts;

        [SerializeField]
        private float m_TextSpeed;

        public string Name => m_Name;
        public string[] Texts => m_Texts;

        public float TextSpeed => m_TextSpeed;
    }
}
