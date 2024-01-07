using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TOwner> where TOwner : MonoBehaviour
{
    TOwner m_Owner;

    public abstract class State
    {
        public StateMachine<TOwner> StateMachine;
        protected TOwner Owner => StateMachine.m_Owner;

        public Dictionary<int, State> Trandision = new();


        public virtual void Init() { }
        public virtual void OnStart(State prevState) { }
        public virtual void OnUpdate() { }
        public virtual void OnExit(State nextState) { }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="owner"></param>
    public StateMachine(TOwner owner) => m_Owner = owner;

    //現在のステートから別のステートにどこからでも遷移したい場合にこれを使用する
    public sealed class AnyState : State { }

    private LinkedList<State> m_States = new();

    public State CurrentState;      //現在のステート
    public State CurrentSubState;   //サブステート
    public State PreviousState;     //前回のステート


    /// <summary>
    /// 初期化後、最初に呼び出すことでステートマシーンが起動する
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    public void Start<TFirst>() where TFirst : State, new()
     => Start(GetOrAddState<TFirst>());

    public void Start(State state)
    {
        CurrentState = state;
        CurrentState.OnStart(null);
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }

        if(CurrentSubState != null)
        {
            CurrentSubState.OnUpdate();
        }
    }

    /// <summary>
    /// ステートを追加する
    /// </summary>
    /// <typeparam name="T">追加したいステート</typeparam>
    /// <returns></returns>
    T Add<T>() where T : State, new()
    {
        T state = new() { StateMachine = this };
        state.Init(); //初期化系の処理をここで行う
        m_States.AddLast(state);
        return state;
    }

    /// <summary>
    /// ステートを取得する
    /// 見つからない場合は新しくステートを追加する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetOrAddState<T>() where T : State, new()
    {
        foreach (State state in m_States)
        {
            if (state is T result) return result;
        }
        return Add<T>();
    }

    /// <summary>
    /// ステートを取得する
    /// </summary>
    /// <typeparam name="T">取得したいステート</typeparam>
    /// <returns></returns>
    T GetState<T>() where T : State, new()
    {
        foreach (State state in m_States)
        {
            if (state is T result) return result;
        }
        return Add<T>();
    }

    /// <summary>
    /// 次のステートに切り替える
    /// </summary>
    /// <param name="nextState">次のステート</param>
    void ChangeState(State nextState)
    {
        CurrentState.OnExit(nextState);
        nextState.OnStart(PreviousState);
        PreviousState = CurrentState;
        CurrentState = nextState;
    }

    /// <summary>
    /// 遷移を新しく追加する
    /// </summary>
    /// <typeparam name="TFrom">元のステート</typeparam>
    /// <typeparam name="TTo">遷移先ステート</typeparam>
    /// <param name="eventId">イベントID</param>
    public void AddTrandision<TFrom, TTo>(int eventId)
        where TFrom : State, new()
        where TTo : State, new()
    {
        TFrom from = GetOrAddState<TFrom>();
        if (from.Trandision.ContainsKey(eventId))
        {
            Debug.LogError($"{nameof(TFrom)}に対して、{eventId}は既に定義済みです");
        }

        TTo to = GetOrAddState<TTo>();
        from.Trandision.Add(eventId, to);
    }

    /// <summary>
    /// どこからでも遷移できるステートを追加
    /// </summary>
    /// <typeparam name="TTo">遷移先ステート</typeparam>
    /// <param name="eventId">イベントID</param>
    public void AddAnyTrandition<TTo>(int eventId) where TTo : State, new()
    {
        AddTrandision<AnyState, TTo>(eventId);
    }

    /// <summary>
    /// メインのステートとは別にサブのステートをアクティブにする
    /// </summary>
    /// <typeparam name="TTo"></typeparam>
    public void SubStateEnable<TTo>() where TTo : State, new()
    {
        TTo state = GetOrAddState<TTo>();
        CurrentSubState = state;
        state.OnStart(null);
    }

    /// <summary>
    /// サブステートを終了する
    /// </summary>
    public void SubStateDisable()
    {
        if (CurrentSubState == null) return;
        CurrentSubState.OnExit(null);
        CurrentSubState = null;
    }

    /// <summary>
    /// 次のStateに遷移する
    /// </summary>
    /// <param name="eventId">イベントID</param>
    public void Dispatch(int eventId)
    {
        if (!CurrentState.Trandision.TryGetValue(eventId, out State to))
        {
            if (!GetOrAddState<AnyState>().Trandision.TryGetValue(eventId, out to))
            {
                //遷移先が見つからない
                Debug.LogWarning("遷移先が見つかりませんでした  イベントID:" + eventId);
                return;
            }
        }

        ChangeState(to);
    }

}
