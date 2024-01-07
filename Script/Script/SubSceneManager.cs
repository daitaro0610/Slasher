using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KD
{
    public class SubSceneManager : MonoBehaviour
    {
        [Header("���̃I�u�W�F�N�g�̓T�u�V�[���ɂ̂ݐݒu���Ă��������B")]
        [SerializeField]
        private SceneData m_SceneData;

        [SerializeField]
        private bool m_IsLightingDisabled;

        public static SubSceneManager instance;
        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            //�C���X�^���X��
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
