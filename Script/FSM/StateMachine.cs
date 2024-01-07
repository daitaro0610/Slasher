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
    /// �R���X�g���N�^
    /// </summary>
    /// <param name="owner"></param>
    public StateMachine(TOwner owner) => m_Owner = owner;

    //���݂̃X�e�[�g����ʂ̃X�e�[�g�ɂǂ�����ł��J�ڂ������ꍇ�ɂ�����g�p����
    public sealed class AnyState : State { }

    private LinkedList<State> m_States = new();

    public State CurrentState;      //���݂̃X�e�[�g
    public State CurrentSubState;   //�T�u�X�e�[�g
    public State PreviousState;     //�O��̃X�e�[�g


    /// <summary>
    /// ��������A�ŏ��ɌĂяo�����ƂŃX�e�[�g�}�V�[�����N������
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
    /// �X�e�[�g��ǉ�����
    /// </summary>
    /// <typeparam name="T">�ǉ��������X�e�[�g</typeparam>
    /// <returns></returns>
    T Add<T>() where T : State, new()
    {
        T state = new() { StateMachine = this };
        state.Init(); //�������n�̏����������ōs��
        m_States.AddLast(state);
        return state;
    }

    /// <summary>
    /// �X�e�[�g���擾����
    /// ������Ȃ��ꍇ�͐V�����X�e�[�g��ǉ�����
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
    /// �X�e�[�g���擾����
    /// </summary>
    /// <typeparam name="T">�擾�������X�e�[�g</typeparam>
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
    /// ���̃X�e�[�g�ɐ؂�ւ���
    /// </summary>
    /// <param name="nextState">���̃X�e�[�g</param>
    void ChangeState(State nextState)
    {
        CurrentState.OnExit(nextState);
        nextState.OnStart(PreviousState);
        PreviousState = CurrentState;
        CurrentState = nextState;
    }

    /// <summary>
    /// �J�ڂ�V�����ǉ�����
    /// </summary>
    /// <typeparam name="TFrom">���̃X�e�[�g</typeparam>
    /// <typeparam name="TTo">�J�ڐ�X�e�[�g</typeparam>
    /// <param name="eventId">�C�x���gID</param>
    public void AddTrandision<TFrom, TTo>(int eventId)
        where TFrom : State, new()
        where TTo : State, new()
    {
        TFrom from = GetOrAddState<TFrom>();
        if (from.Trandision.ContainsKey(eventId))
        {
            Debug.LogError($"{nameof(TFrom)}�ɑ΂��āA{eventId}�͊��ɒ�`�ς݂ł�");
        }

        TTo to = GetOrAddState<TTo>();
        from.Trandision.Add(eventId, to);
    }

    /// <summary>
    /// �ǂ�����ł��J�ڂł���X�e�[�g��ǉ�
    /// </summary>
    /// <typeparam name="TTo">�J�ڐ�X�e�[�g</typeparam>
    /// <param name="eventId">�C�x���gID</param>
    public void AddAnyTrandition<TTo>(int eventId) where TTo : State, new()
    {
        AddTrandision<AnyState, TTo>(eventId);
    }

    /// <summary>
    /// ���C���̃X�e�[�g�Ƃ͕ʂɃT�u�̃X�e�[�g���A�N�e�B�u�ɂ���
    /// </summary>
    /// <typeparam name="TTo"></typeparam>
    public void SubStateEnable<TTo>() where TTo : State, new()
    {
        TTo state = GetOrAddState<TTo>();
        CurrentSubState = state;
        state.OnStart(null);
    }

    /// <summary>
    /// �T�u�X�e�[�g���I������
    /// </summary>
    public void SubStateDisable()
    {
        if (CurrentSubState == null) return;
        CurrentSubState.OnExit(null);
        CurrentSubState = null;
    }

    /// <summary>
    /// ����State�ɑJ�ڂ���
    /// </summary>
    /// <param name="eventId">�C�x���gID</param>
    public void Dispatch(int eventId)
    {
        if (!CurrentState.Trandision.TryGetValue(eventId, out State to))
        {
            if (!GetOrAddState<AnyState>().Trandision.TryGetValue(eventId, out to))
            {
                //�J�ڐ悪������Ȃ�
                Debug.LogWarning("�J�ڐ悪������܂���ł���  �C�x���gID:" + eventId);
                return;
            }
        }

        ChangeState(to);
    }

}
