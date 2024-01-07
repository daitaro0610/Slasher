using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;
using UnityEngine.UI;

public class VerticalSyncSettings : MonoBehaviour
{
    [SerializeField]
    private DropDownController m_DropDownController;

    private VerticalSyncData m_Data;

    private const string DATA_FILE_NAME = "VerticalSyncData";

    private void Awake()
    {
        m_Data = SaveManager.instance.Load(DATA_FILE_NAME, new VerticalSyncData { VirtualSyncCount = 0 });
    }

    private void Start()
    {
        m_DropDownController.SetDropDown(m_Data.VirtualSyncCount);
    }

    public void OnVerticalSyncActive(int vSyncCount)
    {
        m_Data.VirtualSyncCount = vSyncCount;
        SaveManager.instance.Save(m_Data, DATA_FILE_NAME);
        QualitySettings.vSyncCount = vSyncCount;
    }

    public class VerticalSyncData : ISave
    {
        public int VirtualSyncCount;
    }
}