using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KD;
using UnityEngine.InputSystem;

public class TitleController : MonoBehaviour
{
    private PlayerGame m_PlayerGame;

    private bool m_IsStarted;


    private void Awake()
    {
        Time.timeScale = 1;
        m_IsStarted = false;
    }

    private void Start()
    {
        AudioManager.instance.FadeOutBgm(1f);
        AudioManager.instance.PlayBGM(BGMName.Title);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPlay()
    {
        if (m_IsStarted) return;

        AudioManager.instance.FadeInBgm(1f);

        m_IsStarted = true;
        SceneManager.instance.SetAsync(true).PlayLoad(0);
    }

    public void OnEndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}
