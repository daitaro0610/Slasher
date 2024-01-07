using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening;
using TMPro;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private float m_MoveValueY;

    [SerializeField]
    private float m_Duration;
    private TextMeshProUGUI m_Text;

    private Vector3 m_InitialPoint;
    private Vector2 m_MoveValue;

    private Vector2 m_LocalPos;
    private void Awake()
    {
        m_Text = GetComponent<TextMeshProUGUI>();
    }

    public void Init(string text, Vector3 position, Color textColor)
    {
        m_InitialPoint = position;
        m_Text.text = text;
        m_Text.color = textColor;
        m_Text.rectTransform.anchoredPosition = Vector2.zero;

        //Tweenを使用して上に上昇させる
        m_MoveValue = Vector3.zero;
        m_Text.TweenAlpha(0, m_Duration);

        KDTweener.To(
            () => m_MoveValue.y,
            x => m_MoveValue.y = x,
            m_MoveValueY,
            m_Duration
        )
        .SetEase(Ease.OutCirc)
        .OnComplete(() => DamageEffectManager.Instance.Release(this));

        //ワールド座標をスクリーン座標に
        Vector2 screenPos = Camera.main.WorldToScreenPoint(m_InitialPoint);
        //スクリーン座標をUIローカル座標に
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            m_Text.rectTransform,
            screenPos,
            null,
            out Vector2 localPos);
        m_LocalPos = localPos;
        m_Text.rectTransform.anchoredPosition = localPos;
    }

    private void LateUpdate()
    {
        m_Text.rectTransform.anchoredPosition = m_LocalPos + m_MoveValue;
    }
}
