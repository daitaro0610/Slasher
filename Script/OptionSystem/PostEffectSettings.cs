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
    /// ����������
    /// </summary>
    public void Awake()
    {
        m_Data = SaveManager.instance.Load(DATA_FILE_NAME, new PostEffectData
        {
            IsEnable = true,
            Contrast = 0
        });

        m_Volume.profile.TryGetSettings(out m_ColorGradieng);

        //�p�����[�^�̐ݒ�
        m_ColorGradieng.contrast.value = m_Data.Contrast;
        m_Volume.enabled = m_Data.IsEnable;
    }

    /// <summary>
    /// �|�X�g�G�t�F�N�g�̃I���I�t�؂�ւ�����
    /// </summary>
    /// <param name="isbool">true�Ȃ�I���@false�Ȃ�I�t</param>
    public void OnPostEffectEnabled(bool isbool)
    {
        m_Volume.enabled = isbool;
    }

    /// <summary>
    /// �R���g���X�g��ύX���鏈��
    /// </summary>
    /// <param name="value">�R���g���X�g�̒l�@�l���傫���قǈÂ��Ȃ�</param>
    public void OnContrastValueChanged(float value)
    {
        m_ColorGradieng.contrast.value = value;
    }

    public void OnDestroy()
    {
        SaveManager.instance.Save(m_Data, DATA_FILE_NAME);
    }
}

