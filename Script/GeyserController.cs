using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KD.Cinemachine;
using KD.Tweening;

public class GeyserController : MonoBehaviour
{
    [SerializeField]
    private VirtualCamera m_VirtualCamera;

    private Geyser[] m_Geysers;

    [System.Serializable]
    private struct ShakeData
    {
        public float Strength;
        public float Vibrato;
        public float Duration;
    }

    [SerializeField]
    private ShakeData m_ShakeData;

    private void Awake()
    {
        m_Geysers = FindObjectsByType<Geyser>(FindObjectsSortMode.None);
    }

    /// <summary>
    /// すべての間欠泉を作動させる
    /// </summary>
    public void AllGeyserPlay()
    {
        //カメラを振動させる
        //m_VirtualCamera.Shake(m_ShakeData.Strength, m_ShakeData.Vibrato, m_ShakeData.Duration);

        m_VirtualCamera.ShakeCamera(m_ShakeData.Strength, m_ShakeData.Vibrato, m_ShakeData.Duration);
        foreach (var geyser in m_Geysers)
        {
            geyser.GeyserPlay();
        }
    }
}
