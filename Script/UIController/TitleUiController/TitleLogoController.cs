using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Tweening;
using TMPro;

public class TitleLogoController:UiControllerBase
{
    [SerializeField]
    private TextMeshProUGUI m_TitleText;

    [SerializeField]
    private TextMeshProUGUI m_AnyKeyText;

    [SerializeField]
    private float m_MovePositionX = 200; //�^�C�g�����ړ�������ʒu
    [SerializeField]
    private float m_MoveDuration = 1.0f; //�ړ����鎞��

    [SerializeField]
    private float m_ColorDuration = 1.0f;//�F�̕ω��̎��ԁ@�ړ��̎��ԂƓ����ɂ����Ƃ��ɕ\���̎d���Ɉ�a�����������̂ŕʁX��   �@

    public override void OnStart()
    {
        //�^�C�g���𓮂������X�ɕ\�����Ă���
        m_TitleText.TweenAlpha(1f, m_ColorDuration).SetLink(m_TitleText.gameObject);
        m_TitleText.rectTransform.TweenAnchorMoveY(m_MovePositionX, m_MoveDuration).SetLink(m_TitleText.gameObject);

        m_AnyKeyText.TweenAlpha(1f, m_ColorDuration).SetLink(m_AnyKeyText.gameObject);
    }

    public override void OnReset()
    {
        //�����ɂ���
        m_TitleText.TweenAlpha(0f, m_ColorDuration).SetLink(m_TitleText.gameObject);
        m_AnyKeyText.TweenAlpha(0f, m_ColorDuration).SetLink(m_AnyKeyText.gameObject);
    }
}
