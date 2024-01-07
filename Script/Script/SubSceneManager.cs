using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KD
{
    public class SubSceneManager : MonoBehaviour
    {
        [Header("このオブジェクトはサブシーンにのみ設置してください。")]
        [SerializeField]
        private SceneData m_SceneData;

        [SerializeField]
        private bool m_IsLightingDisabled;

        public static SubSceneManager instance;
        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            //インスタンス化
            if (instance == null)
            {
                instance = this;
            }

            if (m_IsLightingDisabled)
            {
                LightmapSettings.lightmaps = new LightmapData[0];
            }

        }

        public void UnLoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            EventSystem.current.enabled = false;
            LightmapSettings.lightmaps = m_SceneData.GetLightMaps();
        }
    }
}
