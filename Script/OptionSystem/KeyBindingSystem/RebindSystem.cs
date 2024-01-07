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
            Debugger.LogWarning($"�Ή����Ă��Ȃ��^�C�v�ł� type:{nameof(T)}");
        }

        return this;
    }

    /// <summary>
    /// �A�N�V������o�^����
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <returns></returns>
    public RebindSystem ChangeInputActionReference(InputActionReference inputActionReference)
    {
        //�o�C���h���Ȃ珈�����Ȃ�
        if (m_IsRebinding) return this;

        m_ActionRef = inputActionReference;
        m_Action = m_ActionRef.action;
        return this;
    }

    /// <summary>
    /// ���o�C���h���s��
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <returns></returns>
    public RebindSystem StartRebind(InputActionReference inputActionReference = null)
    {
        if (inputActionReference != null)
            ChangeInputActionReference(inputActionReference);

        if (m_Action == null) return this;

        //�R�[���o�b�N�����S��
        void OnCallback(UnityAction action)
        {
            if (action == null) return;

            action.Invoke();
            action = null;
        }

        //���ׂĂ̏����̍ŏ��ɍs���R�[���o�b�N
        OnCallback(m_AwakeCallback);

        //���o�C���h���Ȃ璆�f����
        m_RebindingOperation?.Cancel();

        //���o�C���h���̓A�N�V�����𖳌���
        m_Action.Disable();


        //�o�C���h����f�o�C�X���w�肷��
        int bindingIndex = m_Action.GetBindingIndex(InputBinding.MaskByGroup(m_CurrentMaskGroup));

        void OnFinished()
        {
            CleanUpOperation();

            m_Action.Enable();
            m_IsRebinding = false;
        }

        //���o�C���h�̃I�y���[�V�������쐬����
        m_RebindingOperation = m_Action.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(_ => //�I�������ۂ̏���
            {
                OnFinished();

                //�I�����ɌĂ΂��R�[���o�b�N����
                OnCallback(m_CompleteCallback);

            })
            .OnCancel(_ =>  //���f���ꂽ�Ƃ��̏���
            {
                OnFinished();

                //���f���ɌĂ΂��R�[���o�b�N����
                OnCallback(m_CancelCallback);
            })
            .WithCancelingThrough("<Keyboard>/escape")
            .Start();

        //���ׂĂ̏����i�񓯊��܂߂��j���I�������ɌĂ΂��R�[���o�b�N����
        OnCallback(m_StartCallback);
        m_IsRebinding = true;

        return this;
    }

    /// <summary>
    ///���݂̃L�[�o�C���h�\��
    /// </summary>
    /// <returns></returns>
    public string GetBindingDisplayString()
    {
        if (m_Action == null) return null;

        return m_Action.GetBindingDisplayString(group : m_CurrentMaskGroup);
    }


    /// <summary>
    /// �I�y���[�V������j������
    /// </summary>
    private void CleanUpOperation()
    {
        if (m_Action == null) return;

        m_RebindingOperation?.Dispose();
        m_RebindingOperation = null;
    }

    /// <summary>
    /// ���o�C���h�������s����O�ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnAwake(UnityAction action)
    {
        m_AwakeCallback += action;
        return this;
    }

    /// <summary>
    /// �ŏ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnStart(UnityAction action)
    {
        m_StartCallback += action;
        return this;
    }

    /// <summary>
    /// �Ō�ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnComplete(UnityAction action)
    {
        m_CompleteCallback += action;
        return this;
    }

    /// <summary>
    /// ���f���ꂽ�Ƃ��ɌĂ΂��R�[���o�b�N
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public RebindSystem OnCancel(UnityAction action)
    {
        m_CancelCallback += action;
        return this;
    }


}
