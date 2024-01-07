using UnityEngine;
using KD;



public class VolumeSettings : MonoBehaviour
{
    private const string DATA_FILE_NAME = "VolumeData";

    [System.Serializable]
    private struct VolumeData : ISave
    {
        public float MasterVolume;
        public float BgmVolume;
        public float SeVolume;
    }

    private VolumeData m_VolumeData;

    private void Awake()
    {
        m_VolumeData = SaveManager.instance.Load<VolumeData>(DATA_FILE_NAME,
            new VolumeData
            {
                MasterVolume = 0,
                BgmVolume = 0,
                SeVolume = 0
            },
            false);
    }

    private void Start()
    {
        AudioManager.instance.SetAllVolume(m_VolumeData.MasterVolume);
        AudioManager.instance.SetVolumeBgmMixer(m_VolumeData.BgmVolume);
        AudioManager.instance.SetVolumeSeMixer(m_VolumeData.SeVolume);
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetAllVolume(value);
        m_VolumeData.MasterVolume = value;
    }

    public void SetBgmVolume(float value)
    {
        AudioManager.instance.SetVolumeBgmMixer(value);
        m_VolumeData.BgmVolume = value;
    }

    public void SetSeVolume(float value)
    {
        AudioManager.instance.SetVolumeSeMixer(value);
        m_VolumeData.SeVolume = value;
    }

    //îjä¸Ç≥ÇÍÇΩç€Ç…ï€ë∂Ç∑ÇÈ
    public void OnDestroy()
    {
        SaveManager.instance.Save(m_VolumeData, DATA_FILE_NAME);
    }
}
