using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "CreateData/AudioData")]
public class AudioDataBase : ScriptableObject
{
    [Header("BGM�f�[�^")]
    public AudioData[] m_BGMData;

    [Header("SE�f�[�^")]
    public AudioData[] m_SEData;

    [System.Serializable]
    public struct AudioData
    {
        [SerializeField, Tooltip("���O")]
        string m_Name;
        [SerializeField, Tooltip("����")]
        AudioClip m_AudioClip;

        public string Name => m_Name;
        public AudioClip AudioClip => m_AudioClip;
    }
}
