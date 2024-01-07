using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class DisplaySettings : MonoBehaviour
{
    [SerializeField]
    private DropDownController m_DropDownController;

    private Resolution[] m_Resolutions;
    private ScreenData m_Data;

    [SerializeField]
    private int[] m_ResolutionIndexDatas = {21,21,10,8 };

    private const string DATA_FILE_NAME = "ScreenData"; //スクリーンデータのファイル名

    private void Awake()
    {
        m_Resolutions = Screen.resolutions;
        Load();
    }

    private void Start()
    {
        m_DropDownController.SetDropDown(m_Data.ResolutionIndex);
    }

    /// <summary>
    /// 解像度を変更する
    /// </summary>
    /// <param name="index">対応している解像度配列のインデックス値</param>
    public void ChangeResolution(int index)
    {
        if (m_ResolutionIndexDatas.Length <= index) return;

        m_Data.ResolutionIndex = index;
        Apply();
    }

    /// <summary>
    /// フルスクリーンにする場合はTrue
    /// </summary>
    /// <param name="isFullScreen"></param>
    public void FullScreen(bool isFullScreen)
    {
        m_Data.IsFullScreen = isFullScreen;
        Apply();
    }

    /// <summary>
    /// 変更を反映させる
    /// </summary>
    public void Apply()
    {
        Resolution currentResolution = m_Resolutions[m_ResolutionIndexDatas[m_Data.ResolutionIndex]];
        Screen.SetResolution(
            currentResolution.width,
            currentResolution.height,
            m_Data.IsFullScreen);

        Save();
    }

    /// <summary>
    /// スクリーンデータのセーブを行う
    /// </summary>
    private void Save()
    {
        SaveManager.instance.Save(m_Data, DATA_FILE_NAME);
    }

    /// <summary>
    /// スクリーンデータのロードを行う
    /// </summary>
    private void Load()
    {
        m_Data = SaveManager.instance.Load(DATA_FILE_NAME,
            new ScreenData { IsFullScreen = true });
    }
}

[System.Serializable]
public class ScreenData : ISave
{
    public int ResolutionIndex;
    public bool IsFullScreen;
}
