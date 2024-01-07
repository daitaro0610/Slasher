using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KD;

public class ResultTimerTextController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] m_ResultText;

    [SerializeField]
    private TextMeshProUGUI m_RankText;
    private float m_TotalValue;

    [System.Serializable]
    public struct RankingData
    {
        public float Time;
        public string RankText;
        public Color m_Color;
    }

    [SerializeField]
    private RankingData[] m_RankingDatas;

    public void ResultDisplay()
    {
        m_TotalValue = 0;
        for (int i = 0; i < m_ResultText.Length; i++)
        {
            float t = PlayerPrefs.GetFloat(SceneManager.instance.GetSceneName(SceneManager.SceneState.Play, i));
            float minutes = Mathf.FloorToInt(t / 60);
            float seconds = Mathf.FloorToInt(t % 60);
            m_ResultText[i].text = string.Format("{0:00}:{1:00}", minutes, seconds);

            m_TotalValue += t;
        }

        //ƒ‰ƒ“ƒLƒ“ƒO‚ð•\Ž¦‚·‚é
        for (int i = 0; i < m_RankingDatas.Length; i++)
        {
            if (m_RankingDatas[i].Time > m_TotalValue)
            {
                m_RankText.text = m_RankingDatas[i].RankText;
                m_RankText.color = m_RankingDatas[i].m_Color;
                return;
            }
        }
    }
}
