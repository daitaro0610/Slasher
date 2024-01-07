using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KD;

public class ClearController : MonoBehaviour
{
    private PlayerGame m_PlayerGame;

    private bool m_IsStarted;

    private void Awake()
    {
        m_IsStarted = false;

        m_PlayerGame = new();
        //決定ボタンを押したら、タイトルからプレイ状態に変更する
        m_PlayerGame.UI.Submit.started += OnStart;

        m_PlayerGame.Enable();

    }


    public void OnStart(InputAction.CallbackContext context)
    {
        if (m_IsStarted) return;

        m_IsStarted = true;
        SceneManager.instance.TitleLoad();
    }
}
