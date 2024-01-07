using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InformationText : MonoBehaviour
{
    [SerializeField]
    private InputActionReference m_InputActionReference; //�L�[���͂��擾����

    private string m_CurrentMaskGroup;  //�Q�[���p�b�h�ƃL�[�{�[�h�̕\���ؑւ��s��
    private TextMeshProUGUI m_Text;

    [SerializeField]
    private string m_EmptyKeyboardText; //�����\������Ȃ������ꍇ�̌݊�����(�L�[�{�[�h)

    [SerializeField]
    private string m_EmptyGamepadText; //�����\������Ȃ������ꍇ�̌݊�����(�Q�[���p�b�h)

    private enum DeviceState
    {
        Gamepad,
        Keyboard
    }
    private DeviceState m_DeviceState;

    private void Awake()
    {
        m_Text = GetComponent<TextMeshProUGUI>();

        if (Gamepad.current != null)
        {
            m_DeviceState = DeviceState.Gamepad;
        }
        else m_DeviceState = DeviceState.Keyboard;
        ChangeDeviceText();
    }

    void Update()
    {
        switch (m_DeviceState)
        {
            case DeviceState.Gamepad:
                GamepadUpdate();
                break;

            case DeviceState.Keyboard:
                KeyboardUpdate();
                break;
        }
    }

    private void GamepadUpdate()
    {
        if (Gamepad.current == null)
        {
            m_DeviceState = DeviceState.Keyboard;
            ChangeDeviceText();
        }
    }

    private void KeyboardUpdate()
    {
        if(Gamepad.current != null)
        {
            m_DeviceState = DeviceState.Gamepad;
            ChangeDeviceText();
        }
    }

    /// <summary>
    /// �e�L�X�g�ɃL�[��\������
    /// �f�o�C�X�ɂ���ĕ\������L�[��ς���
    /// </summary>
    private void ChangeDeviceText()
    {
        string emptyText = "";
        switch (m_DeviceState)
        {
            case DeviceState.Gamepad:
                m_CurrentMaskGroup = "Gamepad";
                emptyText = m_EmptyGamepadText;
                break;

            case DeviceState.Keyboard:
                m_CurrentMaskGroup = "Keyboard&Mouse";
                emptyText = m_EmptyKeyboardText;
                break;
        }

       string text = m_InputActionReference.action.GetBindingDisplayString(group:m_CurrentMaskGroup);
        
        //���L�[�ŃA�N�V�������@�\����̂��\������
        if(text == "")
            text = emptyText;

        m_Text.text = text;
    }
}
