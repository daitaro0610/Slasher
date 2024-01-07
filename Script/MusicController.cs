using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class MusicController
{
    public static MusicController Instance => m_Instance;
    private static MusicController m_Instance;

    private bool m_IsBattleBgm = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MusicController()
    {
        m_Instance = this;
        m_IsBattleBgm = false;
    }

    public void PlayStageBgm()
    {
        Debugger.Log("Play");
        AudioManager.instance.PlayBGM(BGMName.Stage).FadeOutBgm(1f);
    }
    public void PlayBattleBgm()
    {
        if (m_IsBattleBgm) return;

        m_IsBattleBgm = true;
        AudioManager.instance.FadeOutBgm(1f);
        AudioManager.instance.PlayBGM(BGMName.Battle);
    }

    public void PlayBossBgm()
    {
        AudioManager.instance.FadeOutBgm(5f);
        AudioManager.instance.PlayBGM(BGMName.Boss);
    }

    public void PlayAngryBossBgm()
        => AudioManager.instance.PlayBGM(BGMName.AngryBoss);

    public void PlayClearBgm()
        => AudioManager.instance.PlayBGM(BGMName.GameClear);

    public void StopFadeBgm()
        => AudioManager.instance.FadeInBgm(1f);

    public void PlayGameOverBgm()
        => AudioManager.instance.PlayBGM(BGMName.GameOver);

    public void StopBgm()
        => AudioManager.instance.StopBgm();
}
