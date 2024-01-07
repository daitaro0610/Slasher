using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class GraphicQualitySettings : MonoBehaviour
{
    [SerializeField]
    private DropDownController m_DropDownController;
    [SerializeField]
    private int m_DefaultQulityLevel = 0;

    private GraphicQualityData m_Data;

    private const string DATA_FILE_NAME = "GraphicQualityData";

    private Dictionary<int, int> m_QualityLevelIndex = new()
    {
        {4,0 },
        {3,1 }, 
        {2,2 }, 
        {1,3 },
    };


    private void Awake()
    {
        m_QualityLevelIndex = new()
        {
            { 4, 0 },
            { 3, 1 },
            { 2, 2 },
            { 1, 3 },
        };

        m_Data = SaveManager.instance.Load(DATA_FILE_NAME, new GraphicQualityData { QualityLevel = m_DefaultQulityLevel });
    }

    private void Start()
    {
        int dropDownIndex = m_QualityLevelIndex[m_Data.QualityLevel];
        m_DropDownController.SetDropDown(dropDownIndex);
    }

    public void ChangeQualityLevel(int levelNum)
    {
        m_Data.QualityLevel = levelNum;
        SetQualityLevel(levelNum);
        SaveManager.instance.Save(m_Data,DATA_FILE_NAME);
    }

    private void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }
}

[System.Serializable]
public class GraphicQualityData : ISave
{
    public int QualityLevel;
}
