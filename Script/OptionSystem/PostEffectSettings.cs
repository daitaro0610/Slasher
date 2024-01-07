using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using KD;

public class PostEffectSettings : MonoBehaviour
{
    private const string DATA_FILE_NAME = "PostEffectData";

    [SerializeField]
    private PostProcessVolume m_Volume;
    private ColorGrading m_ColorGradieng;

    public struct PostEffectData : ISave
    {
        public bool IsEnable;
        public float Contrast;
    }
    private PostEffectData m_Data;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Awake()
    {
        m_Data = SaveManager.instance.Load(DATA_FILE_NAME, new PostEffectData
        {
            IsEnable = true,
            Contrast = 0
        });

        m_Volume.profile.TryGetSettings(out m_ColorGradieng);

        //パラメータの設定
        m_ColorGradieng.contrast.value = m_Data.Contrast;
        m_Volume.enabled = m_Data.IsEnable;
    }

    /// <summary>
    /// ポストエフェクトのオンオフ切り替え処理
    /// </summary>
    /// <param name="isbool">trueならオン　falseならオフ</param>
    public void OnPostEffectEnabled(bool isbool)
    {
        m_Volume.enabled = isbool;
    }

    /// <summary>
    /// コントラストを変更する処理
    /// </summary>
    /// <param name="value">コントラストの値　値が大きいほど暗くなる</param>
    public void OnContrastValueChanged(float value)
    {
        m_ColorGradieng.contrast.value = value;
    }

    public void OnDestroy()
    {
        SaveManager.instance.Save(m_Data, DATA_FILE_NAME);
    }
}

