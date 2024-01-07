using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using State = StateMachine<GamePlayUiController>.State;

public class GamePlayUiController : MonoBehaviour
{
    private PlayerGame m_PlayerGame;

    private StateMachine<GamePlayUiController> m_StateMachine;

    [SerializeField]
    private GameObject m_GamePlayOptionObject;
    [SerializeField]
    private GameObject m_GamePlayObject;

    private GamePlayOptionUi m_GamePlayOption;

    public enum Trigger
    {
        Play,
        Option,
    }

    private void Awake()
    {
        GetUiController();
    }

    private void Start()
    {
        //インプットアクション作成
        m_PlayerGame = new();
        SetStateMachine();
        m_PlayerGame.Enable();
    }

    private void GetUiController()
    {
        m_GamePlayOption = m_GamePlayOptionObject.GetComponent<GamePlayOptionUi>();
    }

    private void SetStateMachine()
    {
        m_StateMachine = new StateMachine<GamePlayUiController>(this);

        m_StateMachine.AddTrandision<Option, Play>((int)Trigger.Play);
        m_StateMachine.AddTrandision<Play, Option>((int)Trigger.Option);

        m_StateMachine.Start<Play>();
    }

    // Update is called once per frame
    void Update()
    {
        m_StateMachine.Update();
    }

    class Play : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_GamePlayObject.SetActive(true);
        }

        public override void OnUpdate()
        {

            if (Owner.m_PlayerGame.UI.Menu.WasPressedThisFrame())
            {
                if (Owner.m_GamePlayOption.IsMove && !GameManager.Instance.IsTextDisplayFinished) return;

                    OnMenuDisplay();
            }

        }
        public override void OnExit(State nextState)
        {
            Owner.m_GamePlayObject.SetActive(false);
        }

        private void OnMenuDisplay()
            => Owner.m_StateMachine.Dispatch((int)Trigger.Option);
    }

    class Option : State
    {
        public override void OnStart(State prevState)
        {
            Owner.m_GamePlayOption.gameObject.SetActive(true);
            Owner.m_GamePlayOption.OnStart();
        }

        public override void OnUpdate()
        {
            if (Owner.m_PlayerGame.UI.Cancel.WasPressedThisFrame() && !Owner.m_GamePlayOption.IsMove)
            {
                if (Owner.m_GamePlayOption.IsSelectState())
                    OnMenuHidden();
                else
                    Owner.m_GamePlayOption.SelectTrandidision();
            }

            Owner.m_GamePlayOption.OnUpdate();
        }

        public override void OnExit(State nextState)
        {
            Owner.m_GamePlayOption.OnReset();
        }

        private void OnMenuHidden()
        => Owner.m_StateMachine.Dispatch((int)Trigger.Play);
    }
}
