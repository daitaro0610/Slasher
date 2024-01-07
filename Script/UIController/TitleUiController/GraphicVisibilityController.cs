using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KD.Tweening;

/// <summary>
/// ���g�܂߁A�q��MaskableGraphic���p�������N���X�̕\����\���𐧌䂷��N���X
/// </summary>
public class GraphicVisibilityController : MonoBehaviour
{
    [SerializeField, Header("����Layer�̂ݕ\����\���s��")]
    private LayerMask m_CurrentVisibilityLayer;

    [SerializeField]
    private float m_FadeDuration = 0.5f;

    [SerializeField, Header("����̕\����\��")]
    private bool m_IsAwakeActive = true;
    [SerializeField, Header("������Ԃ���n�߂�ꍇ��True")]
    private bool m_IsAwakeColorClear = false;

    private bool m_IsActive = false;
    public bool IsActive => m_IsActive;

    private MaskableGraphic[] m_Graphics;�@//Image��Text�Ȃǂ�Graphic�֘A
    private Color[] m_DefaultAlphaValue;
    [SerializeField,Header("TiimeScale��0�ł�Fade���邩�ǂ���")]
    private bool m_IsUnscaled = false;

    [Header("�\�������ۂɃI�u�W�F�N�g��I�����邩�ǂ���")]
    public bool IsSelectObjectOnActivate = false;



    [HideInInspector]
    public GameObject SelectObject;

    private void Awake()
    {
        m_Graphics = GetComponentsInChildren<MaskableGraphic>();
        m_Graphics = m_Graphics.Where(x => CompareLayer(m_CurrentVisibilityLayer, x.gameObject.layer)).ToArray();
        Debugger.Log($"�v�f��{m_Graphics.Length}");

        GetDafaultAlphaValue();
        SetActive(m_IsAwakeActive);

        if (m_IsAwakeColorClear)
            SetColorClear();
    }

    /// <summary>
    /// �A���t�@�l�̃f�t�H���g�̒l���擾����
    /// </summary>
    private void GetDafaultAlphaValue()
    {
        m_DefaultAlphaValue = new Color[m_Graphics.Length];

        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_DefaultAlphaValue[i] = m_Graphics[i].color;
        }
    }

    /// <summary>
    /// ���ׂĂ̐F�𓧖��ɂ���
    /// </summary>
    private void SetColorClear()
    {
        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_Graphics[i].color = Color.clear;
        }
    }

    /// <summary>
    /// ���̂܂܈�u�ŏ���
    /// </summary>
    /// <param name="isbool"></param>
    public void SetActive(bool isbool)
    {
        Debugger.Log("�\���ؑ� " + gameObject.name +" : " + isbool);
        m_IsActive = isbool;

        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_Graphics[i].enabled = isbool;
        }

        if (isbool)
            SetSelectObject();
    }

    /// <summary>
    /// Fade���Ȃ��������ꍇ�̏���
    /// </summary>
    /// <param name="isbool"></param>
    public void SetFadeActive(bool isbool)
    {
        //True�̏ꍇ�͕\��������̂Ńt�F�[�h�C������
        if (isbool)
        {
            FadeOut();
            SetSelectObject();
        }
        else//False�̏ꍇ�͔�\���ɂ���
        {
            FadeIn();
        }
    }

    public void SetDuration(float changeDuration)
    {
        m_FadeDuration = changeDuration;
    }

    private void FadeIn()
    {
        m_IsActive = false;
        for (int i = 0; i < m_Graphics.Length; i++)
        {
           var tween = m_Graphics[i].TweenColor(Color.clear, m_FadeDuration);

            if (m_IsUnscaled)
                tween.SetUnscaled();
        }
    }

    private void FadeOut()
    {
        m_IsActive = true;
        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_Graphics[i].enabled = true;
          var tween =  m_Graphics[i].TweenColor(m_DefaultAlphaValue[i], m_FadeDuration);

            if (m_IsUnscaled)
                tween.SetUnscaled();
        }
    }

    private void SetSelectObject()
    {
        if (IsSelectObjectOnActivate && SelectObject != null)
        {
            EventSystem.current.SetSelectedGameObject(SelectObject);
        }
    }

    // LayerMask�ɑΏۂ�Layer���܂܂�Ă��邩�`�F�b�N����
    private bool CompareLayer(LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
}
