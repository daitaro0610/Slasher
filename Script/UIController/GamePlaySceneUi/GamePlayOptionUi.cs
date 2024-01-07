using UnityEngine;
using UnityEngine.UI;
using KD.Tweening;
using UnityEngine.EventSystems;
using State = StateMachine<GamePlayOptionUi>.State;

public class GamePlayOptionUi : UiControllerBase
{
    private StateMachine<GamePlayOptionUi> m_StateMachine;

    private enum Trigger
    {
        Select,
        Camera,
        Display,
        Audio,
    }

    [SerializeField]
    private Image m_SelectAreaBackGround;

    [SerializeField]
    private GameObject m_BackGroundObject;

    [SerializeField]
    private GameObject m_FirstSelectObject;

    [SerializeField]
    private float m_MoveTweenPos = 700;
    [SerializeField]
    private float m_Duration = 1.0f;

    private bool m_IsMove;
    public bool IsMove => m_IsMove;

    private bool m_IsSelectState;

    private CursorSettings m_Cursor;

    #region Buttons

    [SerializeField]
    private Button m_QuitButton;
    [SerializeField]
    private Button m_CameraButton;
    [SerializeField]
    private Button m_DisplayButton;
    [SerializeField]
    private Button m_AudioButton;

    #endregion

    #region GraphicVisibilityController

    [SerializeField]
    private GraphicVisibilityController m_CameraVisibilityController;
    [SerializeField]
    private GraphicVisibilityController m_DisplayVisibilityController;
    [SerializeField]
    private GraphicVisibilityController m_AudioVisibilityController;

    [SerializeField]
    private GraphicVisibilityController[] m_DisplayTabsVisibilityControllers;
    #endregion


    private void Awake()
    {
        m_StateMachine = new(this);
        m_StateMachine.AddTrandision<Select, CameraSetting>((int)Trigger.Camera);
        m_StateMachine.AddTrandision<Select, DisplaySetting>((int)Trigger.Display);
        m_StateMachine.AddTrandision<Select, AudioSettings>((int)Trigger.Audio);
        m_StateMachine.AddAnyTrandition<Select>((int)Trigger.Select);

        m_CameraButton.onClick.AddListener(() => CameraTrandision());
        m_DisplayButton.onClick.AddListener(() => DisplayTrandision());
        m_AudioButton.onClick.AddListener(() => AudioTrandision());

        m_QuitButton.onClick.AddListener(() => GameManager.Instance.OnTitleTrandision());

        m_StateMachine.Start<Select>();

        m_BackGroundObject.SetActive(false);

        m_Cursor = FindAnyObjectByType<CursorSettings>();
    }

    public bool IsSelectState()
    {
        return m_IsSelectState;
    }

    public void SelectTrandidision()
        => m_StateMachine.Dispatch((int)Trigger.Select);

    private void CameraTrandision()
        => m_StateMachine.Dispatch((int)Trigger.Camera);

    private void DisplayTrandision()
        => m_StateMachine.Dispatch((int)Trigger.Display);

    private void AudioTrandision()
        => m_StateMachine.Dispatch((int)Trigger.Audio);


    public override void OnStart()
    {
        Time.timeScale = 0;

        //カーソルの表示
        m_Cursor.IsVisible(true);
        m_IsMove = true;

        m_BackGroundObject.SetActive(true);

        //X軸をm_MoveTweenPosの場所にm_Duration秒で動かす
        m_SelectAreaBackGround.rectTransform.TweenAnchorMoveX(m_MoveTweenPos, m_Duration)
             .SetEase(Ease.OutCubic)
             .SetLink(m_SelectAreaBackGround.gameObject)
             .SetUnscaled()
             .OnComplete(() =>
             {
                 EventSystem.current.SetSelectedGameObject(m_FirstSelectObject);
                 m_IsMove = false;
             });
    }

    public void OnUpdate()
    {
        m_StateMachine.Update();
    }

    public override void OnReset()
    {
        Time.timeScale = 1;


        m_BackGroundObject.SetActive(false);

        //カーソルの非表示
        m_Cursor.IsVisible(false);

        m_IsMove = true;
        EventSystem.current.SetSelectedGameObject(null);

        //横方向に移動する
        m_SelectAreaBackGround.rectTransform.TweenAnchorMoveX(0, m_Duration)
            .SetEase(Ease.OutCubic)
            .SetLink(m_SelectAreaBackGround.gameObject)
            .OnComplete(() =>
            {
                m_IsMove = false;
                gameObject.SetActive(false);
            });
    }

    class Select : State
    {
        public override void OnStart(State prevState)
        {
            EventSystem.current.SetSelectedGameObject(Owner.m_FirstSelectObject);
            Owner.m_IsSelectState = true;
        }

        public override void OnExit(State nextState)
        {
            Owner.m_IsSelectState = false;
        }
    }

    class CameraSetting : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_CameraVisibilityController.SetFadeActive(true);
        }

        public override void OnExit(State nextState)
        {
            Owner.m_CameraVisibilityController.SetFadeActive(false);
        }
    }

    class DisplaySetting : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_DisplayVisibilityController.SetFadeActive(true);
        }

        public override void OnExit(State nextState)
        {
            Owner.m_DisplayVisibilityController.SetFadeActive(false);
            foreach (GraphicVisibilityController visibilityController in Owner.m_DisplayTabsVisibilityControllers)
            {
                visibilityController.SetActive(false);
            }
        }
    }

    class AudioSettings : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_AudioVisibilityController.SetFadeActive(true);
        }

        public override void OnExit(State nextState)
        {
            Owner.m_AudioVisibilityController.SetFadeActive(false);
        }
    }
}
