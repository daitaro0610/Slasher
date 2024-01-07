using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RebindSystem 
{
    private InputActionReference m_ActionRef;
    private InputAction m_Action;
    private InputActionRebindingExtensions.RebindingOperation m_RebindingOperation;

    private string m_CurrentMaskGroup;

    private bool m_IsRebinding = false;
    public bool IsRebinding => m_IsRebinding;

    private UnityAction m_AwakeCallback;
    private UnityAction m_StartCallback;
    private UnityAction m_CompleteCallback;
    private UnityAction m_CancelCallback;

    public void OnDestroy()
    {
        CleanUpOperation();
    }

    public RebindSystem(InputActionReference inputActionReference)
    {
        ChangeInputActionReference(inputActionReference);
        SetDevice<Keyboard>();
    }

    public RebindSystem SetDevice<T>() where T : InputDevice
    {
        if (typeof(T) == typeof(Keyboard))
        {
            m_CurrentMaskGroup = "Keyboard&Mouse";
        }
        else if (typeof(T) == typeof(Mouse))
        {
            m_CurrentMaskGroup = "Keyboard&Mouse";
        }
        else if (typeof(T) == typeof(Gamepad))
        {
            m_CurrentMaskGroup = "Gamepad";
        }
        else
        {
            Debugger.LogWarning($"対応していないタイプです type:{nameof(T)}");
        }

        return this;
    }

    /// <summary>
    /// アクションを登録する
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <returns></returns>
    public RebindSystem ChangeInputActionReference(InputActionReference inputActionReference)
    {
        //バインド中なら処理しない
        if (m_IsRebinding) return this;

        m_ActionRef = inputActionReference;
        m_Action = m_ActionRef.action;
        return this;
    }

    /// <summary>
    /// リバインドを行う
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <returns></returns>
    public RebindSystem StartRebind(InputActionReference inputActionReference = null)
    {
        if (inputActionReference != null)
            ChangeInputActionReference(inputActionReference);

        if (m_Action == null) return this;

        //コールバック処理全般
        void OnCallback(UnityAction action)
        {
            if (action == null) return;

            action.Invoke();
            action = null;
        }

        //すべての処理の最初に行うコールバック
        OnCallback(m_AwakeCallback);

        //リバインド中なら中断する
        m_RebindingOperation?.Cancel();

        //リバインド中はアクションを無効化
        m_Action.Disable();


        //バインドするデバイスを指定する
        int bindingIndex = m_Action.GetBindingIndex(InputBinding.MaskByGroup(m_CurrentMaskGroup));

        void OnFinished()
        {
            CleanUpOperation();

            m_Action.Enable();
            m_IsRebinding = false;
        }

        //リバインドのオペレーションを作成する
        m_RebindingOperation = m_Action.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(_ => //終了した際の処理
            {
                OnFinished();

                //終了時に呼ばれるコールバック処理
                OnCallback(m_CompleteCallback);

            })
            .OnCancel(_ =>  //中断されたときの処理
            {
                OnFinished();

                //中断時に呼ばれるコールバック処理
                OnCallback(m_CancelCallback);
            })
            .WithCancelingThrough("<Keyboard>/escape")
            .Start();

        //すべての処理（非同期含めず）が終わった後に呼ばれるコールバック処理
        OnCallback(m_StartCallback);
        m_IsRebinding = true;

        return this;
    }

    /// <summary>
    ///現在のキーバインド表示
    /// </summary>
    /// <returns></returns>
    public string GetBindingDisplayString()
    {
        if (m_Action == null) return null;

        return m_Action.GetBindingDisplayString(group : m_CurrentMaskGroup);
    }


    /// <summary>
    /// オペレーションを破棄する
    /// </summary>
    private void CleanUpOperation()
    {
        if (m_Action == null) return;

        m_RebindingOperation?.Dispose();
        m_RebindingOperation = null;
    }

    /// <summary>
    /// リバインド処理が行われる前に呼ばれるコールバック
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnAwake(UnityAction action)
    {
        m_AwakeCallback += action;
        return this;
    }

    /// <summary>
    /// 最初に呼ばれるコールバック
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnStart(UnityAction action)
    {
        m_StartCallback += action;
        return this;
    }

    /// <summary>
    /// 最後に呼ばれるコールバック
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnComplete(UnityAction action)
    {
        m_CompleteCallback += action;
        return this;
    }

    /// <summary>
    /// 中断されたときに呼ばれるコールバック
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnCancel(UnityAction action)
    {
        m_CancelCallback += action;
        return this;
    }


}
