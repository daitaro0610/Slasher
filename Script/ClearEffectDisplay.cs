using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Tweening;

public class ClearEffectDisplay : MonoBehaviour
{
    [SerializeField]
    private Image[] m_FrameImage;
    [SerializeField]
    private float m_FrameHeight = 200;
    [SerializeField]
    private float m_FrameMoveDuration = 1f;

    [SerializeField]
    private RectTransform m_ClearIcon;
    private GraphicVisibilityController m_ClearIconVisibilityController;
    private Vector3 m_IconDefaultScale;
    
    [SerializeField]
    private float m_IconMoveDuration = 1f;


    private WaitForSeconds m_WaitTime;
    private void Awake()
    {
        m_WaitTime = new WaitForSeconds(m_FrameMoveDuration);
        m_ClearIconVisibilityController = m_ClearIcon.GetComponent<GraphicVisibilityController>();
        m_IconDefaultScale = m_ClearIcon.localScale;
    }

    public IEnumerator OnClearEffectDisplay()
    {
        foreach (Image img in m_FrameImage)
        {
            img.rectTransform.TweenSizeDelta(new Vector2(img.rectTransform.sizeDelta.x, m_FrameHeight), m_FrameMoveDuration);
        }

        yield return m_WaitTime;

        //リザルトを透明状態から見えるようにする
        m_ClearIconVisibilityController.SetDuration(m_IconMoveDuration / 2);
        m_ClearIconVisibilityController.SetFadeActive(true);

        //スケールを徐々に小さくして元の大きさに戻す
        m_ClearIcon.localScale = Vector3.one * 3;
        m_ClearIcon.TweenScale(m_IconDefaultScale, m_IconMoveDuration).SetEase(Ease.OutQuad);
    }
}
