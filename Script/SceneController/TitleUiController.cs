using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using State = StateMachine<TitleUiController>.State;

public class TitleUiController : MonoBehaviour
{
    private PlayerGame m_PlayerGame;
    private TitleController m_TitleController;

    private StateMachine<TitleUiController> m_StateMachine;

    [SerializeField]
    private GameObject m_TitleLogoObject;

    [SerializeField]
    private GameObject m_SelectObject;

    [SerializeField]
    private GameObject m_HowToObject;

    [SerializeField]
    private GameObject m_OptionObject;

    [SerializeField]
    private GameObject m_CreditObject;

    private float m_EndGameFadeTime = 0.5f;

    #region UiController
    private TitleLogoController m_TitleLogoController;
    private SelectUiController m_SelectUiController;
    private TitleOptionUiController m_OptionUiController;
    private HowToUiController m_HowToUiController;
    private CreditUiController m_CreditUiController;

    #endregion

    public enum Trigger
    {
        Title,
        Select,
        HowTo,
        Option,
        Credit,
    }

    private void Awake()
    {
        //インプットアクション作成
        m_PlayerGame = new();
        m_PlayerGame.Enable();

        var titleControllerObject = GameObject.FindGameObjectWithTag(TagName.TitleController);
        m_TitleController = titleControllerObject.GetComponent<TitleController>();

        GetUiControllers();
    }

    private void GetUiControllers()
    {
        m_TitleLogoController = m_TitleLogoObject.GetComponent<TitleLogoController>();
        m_SelectUiController = m_SelectObject.GetComponent<SelectUiController>();
        m_OptionUiController = m_OptionObject.GetComponent<TitleOptionUiController>();
        m_HowToUiController = m_HowToObject.GetComponent<HowToUiController>();
        m_CreditUiController = m_CreditObject.GetComponent<CreditUiController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_StateMachine = new StateMachine<TitleUiController>(this);

        m_StateMachine.AddTrandision<Title, Select>((int)Trigger.Select);
        m_StateMachine.AddTrandision<Select, Option>((int)Trigger.Option);
        m_StateMachine.AddTrandision<Select, HowTo>((int)Trigger.HowTo);
        m_StateMachine.AddTrandision<Select, Title>((int)Trigger.Title);
        m_StateMachine.AddTrandision<Select, Credit>((int)Trigger.Credit);
        m_StateMachine.AddTrandision<Option, Select>((int)Trigger.Select);
        m_StateMachine.AddTrandision<HowTo, Select>((int)Trigger.Select);
        m_StateMachine.AddTrandision<Credit, Select>((int)Trigger.Select);

        m_StateMachine.Start<Title>();
    }

    // Update is called once per frame
    void Update()
    {
        m_StateMachine.Update();
    }

    class Title : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_TitleLogoController.OnStart();
        }

        public override void OnUpdate()
        {
            if (Owner.m_PlayerGame.UI.Decision.WasPressedThisFrame())
            {
                OnSelectTrandition();
            }
        }

        public override void OnExit(State nextState)
        {
            Owner.m_TitleLogoController.OnReset();
        }

        private void OnSelectTrandition()
        {
            if (Owner.m_SelectUiController.IsMove) return;

            Owner.m_StateMachine.Dispatch((int)Trigger.Select);
        }
    }

    class Select : State
    {
        public override void Init()
        {
            //ボタン関連の設定
            Owner.m_SelectUiController.PlayButton.onClick.AddListener(() => OnPlay());
            Owner.m_SelectUiController.HowToButton.onClick.AddListener(() => OnHowToTrandition());
            Owner.m_SelectUiController.OptionButton.onClick.AddListener(() => OnOptionTrandition());
            Owner.m_SelectUiController.CreditButton.onClick.AddListener(() => OnCreditTrandition());
            Owner.m_SelectUiController.EndGameButton.onClick.AddListener(() => OnEndGame());
        }

        public override void OnStart(State prevState)
        {
            Owner.m_SelectUiController.OnStart();
        }

        public override void OnUpdate()
        {
            if (Owner.m_PlayerGame.UI.Cancel.WasPressedThisFrame())
            {
                OnTitleTrandition();
            }
        }

        public override void OnExit(State nextState)
        {
            Owner.m_SelectUiController.OnReset();
        }

        /// <summary>
        /// プレイシーンに移動する
        /// </summary>
        private void OnPlay()
        {
            Owner.m_TitleController.OnPlay();
        }


        /// <summary>
        /// 操作方法ステートに移動する
        /// </summary>
        private void OnHowToTrandition()
        {
            Owner.m_StateMachine.Dispatch((int)Trigger.HowTo);
        }

        /// <summary>
        /// 設定ステートに移動する
        /// </summary>
        private void OnOptionTrandition()
        {
            Owner.m_StateMachine.Dispatch((int)Trigger.Option);
        }

        /// <summary>
        /// クレジットステートに移動する
        /// </summary>
        private void OnCreditTrandition()
        {
            Owner.m_StateMachine.Dispatch((int)Trigger.Credit);
        }

        private void OnEndGame()
        {
            KD.FadeManager.instance.FadeOut(Owner.m_EndGameFadeTime).OnCompleted(() =>
            {
                Owner.m_TitleController.OnEndGame();
            });
        }

        /// <summary>
        /// タイトルステートに移動する
        /// </summary>
        private void OnTitleTrandition()
        {
            //ここの処理だけUpdate内で行われているため、IsMoveがFalseになるまで待機させる
            if (Owner.m_SelectUiController.IsMove) return;

            Owner.m_StateMachine.Dispatch((int)Trigger.Title);
        }

    }

    class Option : State
    {
        //オプションの項目を選択している場合はTrue
        private bool m_IsSelect = false;


        public override void Init()
        {
            //ステートマシン本体の処理と直結させるためにクリック時のイベントをOptionクラス内で定義
            Owner.m_OptionUiController.Buttons.KeyConfig.onClick.AddListener(() => OnClicked());
            Owner.m_OptionUiController.Buttons.Display.onClick.AddListener(() => OnClicked());
            Owner.m_OptionUiController.Buttons.Audio.onClick.AddListener(() => OnClicked());
        }

        public override void OnStart(State prevState)
        {
            m_IsSelect = false;
            Owner.m_OptionUiController.OnStart();
        }

        public override void OnUpdate()
        {
            if (m_IsSelect)
            {
                SelectUpdate();
            }
            else
            {
                NonSelectUpdate();
            }
        }

        private void SelectUpdate()
        {
            if (Owner.m_PlayerGame.UI.Cancel.WasPressedThisFrame())
            {
                Owner.m_OptionUiController.OnLeave();
                m_IsSelect = false;
            }
        }

        private void NonSelectUpdate()
        {
            if (Owner.m_PlayerGame.UI.Cancel.WasPressedThisFrame())
            {
                OnSelectTrandition();
            }
        }
        private void OnClicked()
        {
            m_IsSelect = true;
        }

        public override void OnExit(State nextState)
        {
            Owner.m_OptionUiController.OnReset();
        }

        private void OnSelectTrandition()
        {
            Owner.m_StateMachine.Dispatch((int)Trigger.Select);
        }
    }

    class HowTo : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_HowToUiController.OnStart();
        }

        public override void OnUpdate()
        {
            if (Owner.m_PlayerGame.UI.Cancel.WasPressedThisFrame())
            {
                OnSelectTrandition();
            }
        }
        public override void OnExit(State nextState)
        {
            Owner.m_HowToUiController.OnReset();
        }

        private void OnSelectTrandition()
        {
            Owner.m_StateMachine.Dispatch((int)Trigger.Select);
        }
    }

    class Credit : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_CreditUiController.OnStart();
        }

        public override void OnUpdate()
        {
            if (Owner.m_PlayerGame.UI.Cancel.WasPressedThisFrame())
            {
                OnSelectTrandition();
            }
        }

        public override void OnExit(State nextState)
        {
            Owner.m_CreditUiController.OnReset();
        }

        private void OnSelectTrandition()
        {
            if (Owner.m_SelectUiController.IsMove) return;

            Owner.m_StateMachine.Dispatch((int)Trigger.Select);
        }
    }
}
