using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KD.Tweening;
using KD;

public class TitleOptionUiController : UiControllerBase
{
    [SerializeField]
    private Image m_SelectImageBackGround;

    [SerializeField]
    private GameObject m_FirstSelectObject;

    /// <summary>
    /// ボタンのデータを格納する構造体
    /// </summary>
    [System.Serializable]
    public struct ButtonData
    {
        [SerializeField]
        private Button m_KeyConfigButton;
        public Button KeyConfig => m_KeyConfigButton;
        [SerializeField]
        private Button m_DisplayButton;
        public Button Display => m_DisplayButton;
        [SerializeField]
        private Button m_AudioButton;
        public Button Audio => m_AudioButton;
    }
    [SerializeField]
    private ButtonData m_ButtonData;
    public ButtonData Buttons => m_ButtonData;
    //最後に押したボタンオブジェクトを記憶しておく
    private GameObject m_MemoryClickedButtonObject;

    /// <summary>
    /// 表示非表示を行うスクリプトを格納する構造体
    /// </summary>
    [System.Serializable]
    public struct GraphicVisibilityControllerData
    {
        public GraphicVisibilityController KeyConfig;
        public GraphicVisibilityController Display;
        public GraphicVisibilityController Audio;
    }
    [SerializeField]
    private GraphicVisibilityControllerData m_GraphicVisibilityControllers;

    private struct CanvasData
    {
        public Canvas KeyConfig;
        public Canvas Display;
        public Canvas Audio;
    }

    private CanvasData m_CanvasData;

    [SerializeField]
    private float m_MoveValue = 700f;
    [SerializeField]
    private float m_Duration = 1.0f;

    private enum SortState
    {
        KeyConfig,
        Display,
        Audio,
        None
    }

    private void Awake()
    {
        m_SelectImageBackGround.rectTransform.sizeDelta = new Vector2(m_MoveValue, 0);

        //それぞれCanvasを取得する
        m_CanvasData = new CanvasData
        {
            Audio = m_GraphicVisibilityControllers.Audio.GetComponent<Canvas>(),
            Display = m_GraphicVisibilityControllers.Display.GetComponent<Canvas>(),
            KeyConfig = m_GraphicVisibilityControllers.KeyConfig.GetComponent<Canvas>()
        };

        //ボタンのイベントを設定
        m_ButtonData.KeyConfig.onClick.AddListener(() => OnKeyConfigClicked());
        m_ButtonData.Display.onClick.AddListener(() => OnDisplayClicked());
        m_ButtonData.Audio.onClick.AddListener(() => OnAudioClicked());
    }

    public override void OnStart()
    {
        m_SelectImageBackGround.rectTransform.TweenAnchorMoveX(m_MoveValue, m_Duration).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(m_FirstSelectObject);
            });
    }

    #region ClickEvent

    /// <summary>
    /// KeyConfigのボタンが押されたときの処理
    /// </summary>
    private void OnKeyConfigClicked()
    {
        OnClick();
        CanvasSortChange(SortState.KeyConfig);
        m_GraphicVisibilityControllers.KeyConfig.SetFadeActive(true);
        m_MemoryClickedButtonObject = m_ButtonData.KeyConfig.gameObject;
    }

    /// <summary>
    /// Displayのボタンが押されたときの処理
    /// </summary>
    private void OnDisplayClicked()
    {
        OnClick();
        CanvasSortChange(SortState.Display);
        m_GraphicVisibilityControllers.Display.SetFadeActive(true);
        m_MemoryClickedButtonObject = m_ButtonData.Display.gameObject;
    }

    /// <summary>
    /// Audioのボタンが押されたときの処理
    /// </summary>
    private void OnAudioClicked()
    {
        OnClick();
        CanvasSortChange(SortState.Audio);
        m_GraphicVisibilityControllers.Audio.SetFadeActive(true);
        m_MemoryClickedButtonObject = m_ButtonData.Audio.gameObject;
    }

    private void OnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion

    /// <summary>
    /// 選択している状態から元の状態に戻す
    /// </summary>
    public void OnLeave()
    {
        AllGraphicSetActive(false);

        EventSystem.current.SetSelectedGameObject(m_MemoryClickedButtonObject);
    }

    public override void OnReset()
    {
        EventSystem.current.SetSelectedGameObject(null);
        //CanvasのSortの値を元に戻す
        CanvasSortChange(SortState.None);

        //横方向に移動する
        m_SelectImageBackGround.rectTransform.TweenAnchorMoveX(0, m_Duration).SetEase(Ease.OutCubic);
    }

    private void CanvasSortChange(SortState state)
    {
        m_CanvasData.KeyConfig.sortingOrder = 0;
        m_CanvasData.Display.sortingOrder = 0;
        m_CanvasData.Audio.sortingOrder = 0;

        switch (state)
        {
            case SortState.KeyConfig:
                m_CanvasData.KeyConfig.sortingOrder = 5;
                break;
            case SortState.Display:
                m_CanvasData.Display.sortingOrder = 5;
                break;
            case SortState.Audio:
                m_CanvasData.Audio.sortingOrder = 5;
                break;
            default: break;
        }
    }
    private void AllGraphicSetActive(bool isActive)
    {
        if (m_GraphicVisibilityControllers.KeyConfig.IsActive)
            m_GraphicVisibilityControllers.KeyConfig.SetFadeActive(isActive);

        if (m_GraphicVisibilityControllers.Display.IsActive)
            m_GraphicVisibilityControllers.Display.SetFadeActive(isActive);

        if (m_GraphicVisibilityControllers.Audio.IsActive)
            m_GraphicVisibilityControllers.Audio.SetFadeActive(isActive);
    }
}
