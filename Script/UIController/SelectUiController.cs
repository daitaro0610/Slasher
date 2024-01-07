using UnityEngine;
using UnityEngine.UI;
using KD.Tweening;
using KD;
using UnityEngine.EventSystems;

public class SelectUiController : UiControllerBase
{
    [SerializeField]
    private Image m_SelectBackGroundImage;
    [SerializeField]
    private GameObject m_FirstSelectObject;

    [SerializeField]
    private float m_MoveTweenPos = 700f;
    [SerializeField]
    private float m_Duration = 1.0f;

    private bool m_IsMove;
    public bool IsMove => m_IsMove;

    #region Buttons

    [SerializeField]
    private Button m_PlayButton;
    public Button PlayButton => m_PlayButton;
    [SerializeField]
    private Button m_HowToButton;
    public Button HowToButton => m_HowToButton;
    [SerializeField]
    private Button m_OptionButton;
    public Button OptionButton => m_OptionButton;
    [SerializeField]
    private Button m_CreditButton;
    public Button CreditButton => m_CreditButton;
    [SerializeField]
    private Button m_EndGameButton;
    public Button EndGameButton => m_EndGameButton;

    #endregion

    private void Awake()
    {
        m_SelectBackGroundImage.rectTransform.sizeDelta = new Vector2(m_MoveTweenPos, 0);
    }

    public override void OnStart()
    {
        m_IsMove = true;

        //XŽ²‚ðm_MoveTweenPos‚ÌêŠ‚Ém_Duration•b‚Å“®‚©‚·
        m_SelectBackGroundImage.rectTransform.TweenAnchorMoveX(m_MoveTweenPos, m_Duration)
             .SetEase(Ease.OutCubic)
             .SetLink(m_SelectBackGroundImage.gameObject)
             .OnComplete(() =>
             {
                 EventSystem.current.SetSelectedGameObject(m_FirstSelectObject);
                 m_IsMove = false;
             });
    }

    public override void OnReset()
    {
        m_IsMove = true;
        EventSystem.current.SetSelectedGameObject(null);

        //‰¡•ûŒü‚ÉˆÚ“®‚·‚é
        m_SelectBackGroundImage.rectTransform.TweenAnchorMoveX(0, m_Duration)
            .SetEase(Ease.OutCubic)
            .SetLink(m_SelectBackGroundImage.gameObject)
            .OnComplete(() =>
            {
                m_IsMove = false;
            });
    }
}
