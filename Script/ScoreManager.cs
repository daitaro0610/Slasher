using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;
public class ScoreManager
{
    public ScoreManager(int SceneIndex,float time)
    {
        Debugger.Log($"スコアの保存  index:{SceneIndex}  Time: {time}");
        PlayerPrefs.SetFloat(SceneManager.instance.GetSceneName(SceneManager.SceneState.Play,SceneIndex),time);
    }
}