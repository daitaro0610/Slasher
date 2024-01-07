using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;
using UnityEngine.InputSystem;
using KD.Cinemachine;

public class CameraSettings : MonoBehaviour
{
    public CameraSettingsData m_Data;
    public const string DATA_FILE_NAME = "CameraSettingsData";

    [SerializeField]
    private VirtualCamera m_VirtualCamera;

    private void Awake()
    {
        m_Data = SaveManager.instance.Load(DATA_FILE_NAME,
             new CameraSettingsData
             {
                 IsHorizontalInvert = false,
                 IsVerticalInvert = false,
                 m_Intensity = 1f
             });
        SetHorizontalInvert(m_Data.IsHorizontalInvert);
        SetVerticalInvert(m_Data.IsVerticalInvert);
        SetIntensity(m_Data.m_Intensity);
    }

    public void SetHorizontalInvert(bool isInvert)
    {
        m_Data.IsHorizontalInvert = isInvert;
        m_VirtualCamera.Aim.m_HorizontalAxis.m_Invert = isInvert;
    }

    public void SetVerticalInvert(bool isInvert)
    {
        m_Data.IsVerticalInvert = isInvert;
        m_VirtualCamera.Aim.m_VerticalAxis.m_Invert = isInvert;
    }

    public void SetIntensity(float value)
    {
        m_Data.m_Intensity = value;
        m_VirtualCamera.SetIntensity(value);
    }

    public void OnDestroy()
    {
        SaveManager.instance.Save(m_Data, DATA_FILE_NAME);
    }
}

public struct CameraSettingsData : ISave
{
    public bool IsHorizontalInvert;
    public bool IsVerticalInvert;
    public float m_Intensity;
}
