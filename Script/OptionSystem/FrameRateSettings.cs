using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class FrameRateSettings : MonoBehaviour
{
    [SerializeField]
    private DropDownController m_DropDonwController;

    [SerializeField]
    private int[] m_FrameRates = { 120, 60, 30 };

    private FrameRateData m_Data;

    private const string DATA_FILE_NAME = "FrameRateData"; //スクリーンデータのファイル名

    private void Awake()
    {
        Load();
    }
    private void Start()
    {
        m_DropDonwController.SetDropDown(m_Data.FrameRateIndex);
    }

    private void Load()
    {
        m_Data = SaveManager.instance.Load(DATA_FILE_NAME, new FrameRateData { FrameRateIndex = 0 });
        SetFrameRate(m_Data.FrameRateIndex);
    }

    public void SetFrameRate(int index)
    {
        if (m_FrameRates.Length <= index) return;

        Application.targetFrameRate = m_FrameRates[index];
        m_Data.FrameRateIndex = index;

        Save();
    }

    private void Save()
    {
        SaveManager.instance.Save(m_Data, DATA_FILE_NAME);
    }
}

[System.Serializable]
public class FrameRateData : ISave
{
    public int FrameRateIndex;
}
