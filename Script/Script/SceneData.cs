using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateData/SceneData", fileName = "SceneData")]
public class SceneData : ScriptableObject
{
    [SerializeField, Header("基本的なシーン")]
    private Object m_TitleScene;

    [SerializeField]
    private Object[] m_PlayScene;

    [SerializeField]
    private Object m_GameClearScene;

    [SerializeField]
    private Object m_GameOverScene;

    [SerializeField, Header("追加のシーン")]
    private Object[] m_OtherScene;

    [SerializeField, Header("サブシーン")]
    private Object[] m_SubScene;

    [SerializeField, Header("フェードするかどうか")]
    private bool isFade;
    [SerializeField]
    private float m_FadeTime;

    [Space(20)]
    [ReadOnly]
    public string TitleScene = "";
    [ReadOnly]
    public List<string> PlayScene = new();
    [ReadOnly]
    public string GameClearScene = "";
    [ReadOnly]
    public string GameOverScene = "";
    [ReadOnly]
    public List<string> OtherScene = new();
    [ReadOnly]
    public List<string> SubScene = new();
    public bool IsFade => isFade;
    public float FadeTime => m_FadeTime;

    private LightmapData[] LightMaps;

    public void SaveName()
    {
        if (m_TitleScene != null)
            TitleScene = m_TitleScene.name;

        for (int i = 0; i < m_PlayScene.Length; i++)
        {
            if (m_PlayScene[i] == null)
                continue;

            if (i >= PlayScene.Count)
            {
                PlayScene.Add(m_PlayScene[i].name);
            }
            else PlayScene[i] = m_PlayScene[i].name;
        }

        if (m_GameClearScene != null)
            GameClearScene = m_GameClearScene.name;

        if (m_GameOverScene != null)
            GameOverScene = m_GameOverScene.name;

        for (int i = 0; i < m_OtherScene.Length; i++)
        {
            if (m_OtherScene[i] == null)
                continue;

            if (i >= OtherScene.Count)
            {
                OtherScene.Add(m_OtherScene[i].name);
            }
            else OtherScene[i] = m_OtherScene[i].name;
        }

        for (int i = 0; i < m_SubScene.Length; i++)
        {
            if (m_SubScene[i] == null)
                continue;

            if (i >= SubScene.Count)
            {
                SubScene.Add(m_SubScene[i].name);
            }
            else SubScene[i] = m_SubScene[i].name;
        }
    }

    public LightmapData[] GetLightMaps() => LightMaps;

    public void SaveLightMap(LightmapData[] lightmaps)
    {
        // サブシーンのLightmapDataコンポーネントに元のシーンのライトマップデータを設定
        LightmapData[] currentLightmaps = new LightmapData[lightmaps.Length];
        for (int i = 0; i < lightmaps.Length; i++)
        {
            currentLightmaps[i] = lightmaps[i];
        }

        LightMaps = currentLightmaps;
    }
}
