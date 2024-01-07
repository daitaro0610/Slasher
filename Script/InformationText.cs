using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InformationText : MonoBehaviour
{
    [SerializeField]
    private InputActionReference m_InputActionReference; //キー入力を取得する

    private string m_CurrentMaskGroup;  //ゲームパッドとキーボードの表示切替を行う
    private TextMeshProUGUI m_Text;

    [SerializeField]
    private string m_EmptyKeyboardText; //何も表示されなかった場合の互換文字(キーボード)

    [SerializeField]
    private string m_EmptyGamepadText; //何も表示されなかった場合の互換文字(ゲームパッド)

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
    /// テキストにキーを表示する
    /// デバイスによって表示するキーを変える
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
        
        //何キーでアクションが機能するのか表示する
        if(text == "")
            text = emptyText;

        m_Text.text = text;
    }
}
