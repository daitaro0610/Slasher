using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace KD
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager instance;

        [SerializeField]
        private SceneData m_SceneData;

        public static bool IsLoadScene;

        private bool isFade;
        private bool IsUnscale = true;

        private int m_SceneNum;

        [SerializeField]
        private int m_PlaySceneDefaultNum = 0;
        private int m_CurrentPlaySceneNum;
        public int CurrentPlaySceneNum => m_CurrentPlaySceneNum;

        [HideInInspector]
        public float asyncprogress = 0.0f;

        private AsyncOperation operation_ = null;

        [SerializeField, Header("�V�[�������O�ɓǂݍ��ނ��ǂ���")]
        private bool IsAsyncLoadScene = false;

        [SerializeField, Header("�G�f�B�^��ŃV�[����؂�ւ��鏈���̃I���I�t")]
        private bool IsChangeSceneEditor = true;
        [SerializeField, Header("�G�f�B�^��ŃV�[����؂�ւ���Ƃ��̃L�[")]
        private KeyCode m_ChangeSceneKey = KeyCode.Space;


        public enum SceneState
        {
            Title,
            Play,
            Clear,
            Over,
        }

        private void Awake()
        {
            //�C���X�^���X��
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                m_SceneNum = 0;
                IsLoadScene = false;
                isFade = m_SceneData.IsFade;
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                LoadScene(SceneState.Play);
            }

            if (Input.GetKeyDown(m_ChangeSceneKey) && !IsLoadScene && IsChangeSceneEditor)
            {
                for (int i = 0; i < 4 + m_SceneData.OtherScene.Count; i++)
                {
                    if (m_SceneNum == i)
                    {
                        switch (i)
                        {
                            case 0: LoadScene(SceneState.Title); break;
                            case 1: LoadScene(SceneState.Play); break;
                            case 2: LoadScene(SceneState.Clear); break;
                            case 3: LoadScene(SceneState.Over); break;
                            default: LoadScene(i - 4); break;
                        }
                    }
                }

                m_SceneNum++;
                if (4 + m_SceneData.OtherScene.Count <= m_SceneNum)
                {
                    m_SceneNum = 0;
                }
            }

            if (operation_ != null)
            {
                asyncprogress = operation_.progress;
            }
        }

        /// <summary>
        /// ���O���[�h���s�����ǂ���
        /// </summary>
        /// <param name="isbool"></param>
        /// <returns></returns>
        public SceneManager SetAsync(bool isbool)
        {
            IsAsyncLoadScene = isbool;
            return this;
        }

        /// <summary>
        /// �^�C�g���V�[����ǂݍ���
        /// </summary>
        /// <returns></returns>
        public SceneManager TitleLoad()
        {
            if (IsLoadScene) return this;
            IsLoadScene = true;

            if (IsAsyncLoadScene)
            {
                StartCoroutine(LoadCotoutine(m_SceneData.TitleScene));
            }
            else
            {
                if (isFade)
                {
                    FadeManager.instance.FadeOut(m_SceneData.FadeTime, IsUnscale).OnCompleted(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.TitleScene);
                    });
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.TitleScene);
                }

            }
            return this;
        }

        private IEnumerator LoadCotoutine(string name)
        {
            // �񓯊��Ń��[�h���s��
            operation_ = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);

            // ���[�h���������Ă��Ă��C�V�[���̃A�N�e�B�u���͋����Ȃ�
            operation_.allowSceneActivation = false;


            if (isFade)
            {
                FadeManager.instance.FadeOut(m_SceneData.FadeTime, IsUnscale).OnCompleted(() =>
                {
                    StartCoroutine(WaitActivation());
                });
                yield break;
            }

            yield return WaitActivation();
        }

        private IEnumerator WaitActivation()
        {
            // ���[�h����������܂őҋ@����
            while (operation_.progress < 0.9f)
            {
                Debugger.Log(operation_.progress);
                yield return null;
            }

            // ���[�h�����������Ƃ��ɃV�[���̃A�N�e�B�u����������
            operation_.allowSceneActivation = true;
        }

        /// <summary>
        /// �v���C�V�[����ǂݍ���
        /// </summary>
        /// <returns></returns>
        public SceneManager PlayLoad(int num)
        {
            if (IsLoadScene) return this;
            IsLoadScene = true;
            m_CurrentPlaySceneNum = num;

            if (IsAsyncLoadScene)
            {
                StartCoroutine(LoadCotoutine(m_SceneData.PlayScene[num]));
            }
            else
            {
                if (isFade)
                {
                    FadeManager.instance.FadeOut(m_SceneData.FadeTime, IsUnscale).OnCompleted(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.PlayScene[num]);
                    });
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.PlayScene[num]);
                }

            }
            return this;
        }

        /// <summary>
        /// ����PlayScene�ɑJ�ڂ���
        /// PlayScene�̗v�f���𒴂����ꍇ�̓N���A�V�[���Ɉړ�����
        /// </summary>
        /// <returns></returns>
        public SceneManager NextScene()
        {
            if (m_SceneData.PlayScene.Count > m_CurrentPlaySceneNum + 1)
            {
                PlayLoad(m_CurrentPlaySceneNum + 1);
            }
            else
            {
                ClearLoad();
            }

            return this;
        }

        /// <summary>
        /// �N���A�V�[����ǂݍ���
        /// </summary>
        /// <returns></returns>
        public SceneManager ClearLoad()
        {
            if (IsLoadScene) return this;
            IsLoadScene = true;
            if (IsAsyncLoadScene)
            {
                StartCoroutine(LoadCotoutine(m_SceneData.GameClearScene));
            }
            else
            {
                if (isFade)
                {
                    FadeManager.instance.FadeOut(m_SceneData.FadeTime, IsUnscale).OnCompleted(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.GameClearScene);
                    });
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.GameClearScene);
                }

            }
            return this;
        }

        /// <summary>
        /// �Q�[���I�[�o�[�V�[����ǂݍ���
        /// </summary>
        /// <returns></returns>
        public SceneManager OverLoad()
        {
            if (IsLoadScene) return this;
            IsLoadScene = true;
            if (IsAsyncLoadScene)
            {
                StartCoroutine(LoadCotoutine(m_SceneData.GameOverScene));
            }
            else
            {
                if (isFade)
                {
                    FadeManager.instance.FadeOut(m_SceneData.FadeTime, IsUnscale).OnCompleted(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.GameOverScene);
                    });
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.GameOverScene);
                }

            }
            return this;
        }

        /// <summary>
        /// SceneData���̔C�ӂ�OtherScene�̓ǂݍ��݁@
        /// </summary>
        /// <param name="num">OtherScene�̔z��ԍ�</param>
        /// <returns></returns>
        public SceneManager LoadScene(int num)
        {
            if (IsLoadScene) return this;
            IsLoadScene = true;
            if (IsAsyncLoadScene)
            {
                StartCoroutine(LoadCotoutine(m_SceneData.OtherScene[num]));
            }
            else
            {
                if (isFade)
                {
                    FadeManager.instance.FadeOut(m_SceneData.FadeTime, IsUnscale).OnCompleted(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.OtherScene[num]);
                    });
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneData.OtherScene[num]);
                }

            }
            return this;
        }

        /// <summary>
        /// �v���C�V�[����ǂݍ��ލۂ̐؂�ւ���V�[���̗v�f��
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public SceneManager SetPlayDefaultScene(int num)
        {
            m_PlaySceneDefaultNum = num;
            return this;
        }
        /// <summary>
        /// �V�[����C�ӂ�OtherScene�ȊO��ǂݍ���
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public SceneManager LoadScene(SceneState scene)
        {
            switch (scene)
            {
                case SceneState.Title: TitleLoad(); break;
                case SceneState.Play: PlayLoad(m_PlaySceneDefaultNum); break;
                case SceneState.Over: OverLoad(); break;
                case SceneState.Clear: ClearLoad(); break;
            }

            return this;
        }

        private Scene m_SubScene;

        /// <summary>
        /// �T�u�V�[����\������
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public SceneManager SubSceneLoad(int num)
        {
            SetFade(false);
            m_SceneData.SaveLightMap(LightmapSettings.lightmaps);

            StartCoroutine(LoadSubScene(num));

            return this;
        }

        /// <summary>
        /// �T�u�V�[���̓ǂݍ��݂�񓯊��ōs��
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private IEnumerator LoadSubScene(int num)
        {
            // �T�u�V�[����񓯊��Ń��[�h
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_SceneData.SubScene[num], LoadSceneMode.Additive);

            // ���[�h����������܂őҋ@
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // �T�u�V�[�����擾
            m_SubScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(m_SceneData.SubScene[num]);

            //�C�x���g�V�X�e���̌��݂̃V�[���̑I�����Ă���I�u�W�F�N�g�𖳌��ɂ���
            EventSystem.current.enabled = false;

            // �T�u�V�[�����A�N�e�B�u�ɂ���
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(m_SubScene);
        }

        /// <summary>
        /// �t�F�[�h���邩�����߂�
        /// </summary>
        /// <param name="fade">�t�F�[�h�����邩�ǂ���</param>
        /// <param name="unScale">TimeScale��0�ł��t�F�[�h���邩�ǂ���</param>
        /// <returns></returns>
        public SceneManager SetFade(bool fade, bool unScale = true)
        {
            isFade = fade;
            IsUnscale = unScale;
            return this;
        }


        /// <summary>
        /// �V�[�����ǂݍ��܂ꂽ��Ă΂��
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            operation_ = null;
            asyncprogress = 0;

            if (m_SceneData.IsFade)
            {
                FadeManager.instance.FadeObjAlphaSetUp(1.0f);
                _ = Wait.WaitTime(0.3f, () => FadeManager.instance.FadeIn(m_SceneData.FadeTime, IsUnscale).OnCompleted(() => IsLoadScene = false));
            }
            else
            {
                IsLoadScene = false;
            }
        }

        public string GetSceneName(SceneState sceneState , int index = 0)
        {
            return sceneState switch
            {
                SceneState.Title => m_SceneData.TitleScene,
                SceneState.Play  => m_SceneData.PlayScene[index],
                SceneState.Clear => m_SceneData.GameOverScene,
                SceneState.Over  => m_SceneData.GameOverScene,
                _ => "",
            };
        }
    }
}
