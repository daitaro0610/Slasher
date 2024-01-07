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
    private float m_MovePositionX = 200; //タイトルを移動させる位置
    [SerializeField]
    private float m_MoveDuration = 1.0f; //移動する時間

    [SerializeField]
    private float m_ColorDuration = 1.0f;//色の変化の時間　移動の時間と同じにしたときに表示の仕方に違和感があったので別々に   　

    public override void OnStart()
    {
        //タイトルを動かしつつ徐々に表示していく
        m_TitleText.TweenAlpha(1f, m_ColorDuration).SetLink(m_TitleText.gameObject);
        m_TitleText.rectTransform.TweenAnchorMoveY(m_MovePositionX, m_MoveDuration).SetLink(m_TitleText.gameObject);

        m_AnyKeyText.TweenAlpha(1f, m_ColorDuration).SetLink(m_AnyKeyText.gameObject);
    }

    public override void OnReset()
    {
        //透明にする
        m_TitleText.TweenAlpha(0f, m_ColorDuration).SetLink(m_TitleText.gameObject);
        m_AnyKeyText.TweenAlpha(0f, m_ColorDuration).SetLink(m_AnyKeyText.gameObject);
    }
}
