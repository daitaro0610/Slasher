using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using KD.Tweening;

public class ColorGradingController : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume m_Volume;
    private ColorGrading m_ColorGradieng;

    [SerializeField]
    private float m_RedValue;

    [SerializeField]
    private float m_Duration;

    private float m_DefaultRedValue;

    private void Awake()
    {
        m_Volume.profile.TryGetSettings(out m_ColorGradieng);
        m_DefaultRedValue = m_ColorGradieng.mixerRedOutRedIn.value;
    }

    /// <summary>
    /// âÊñ Çê‘ÇﬂÇ…Ç∑ÇÈ
    /// </summary>
    public void ApplyRedTintUsingChannelMixer()
    {
        KDTweener.To(() => m_ColorGradieng.mixerRedOutRedIn.value,
            (x) => m_ColorGradieng.mixerRedOutRedIn.value = x,
            m_RedValue,
            m_Duration)
            .SetEase(Ease.OutCirc);
    }

    private void OnDestroy()
    {
        m_ColorGradieng.mixerRedOutRedIn.value = m_DefaultRedValue;
    }
}
