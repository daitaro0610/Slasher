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

        [SerializeField, Header("シーンを事前に読み込むかどうか")]
        private bool IsAsyncLoadScene = false;

        [SerializeField, Header("エディタ上でシーンを切り替える処理のオンオフ")]
        private bool IsChangeSceneEditor = true;
        [SerializeField, Header("エディタ上でシーンを切り替えるときのキー")]
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
            //インスタンス化
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
        /// 事前ロードを行うかどうか
        /// </summary>
        /// <param name="isbool"></param>
        /// <returns></returns>
        public SceneManager SetAsync(bool isbool)
        {
            IsAsyncLoadScene = isbool;
            return this;
        }

        /// <summary>
        /// タイトルシーンを読み込む
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
            // 非同期でロードを行う
            operation_ = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);

            // ロードが完了していても，シーンのアクティブ化は許可しない
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
            // ロードが完了するまで待機する
            while (operation_.progress < 0.9f)
            {
                Debugger.Log(operation_.progress);
                yield return null;
            }

            // ロードが完了したときにシーンのアクティブ化を許可する
            operation_.allowSceneActivation = true;
        }

        /// <summary>
        /// プレイシーンを読み込む
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
        /// 次のPlaySceneに遷移する
        /// PlaySceneの要素数を超えた場合はクリアシーンに移動する
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
        /// クリアシーンを読み込む
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
        /// ゲームオーバーシーンを読み込む
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
        /// SceneData内の任意のOtherSceneの読み込み　
        /// </summary>
        /// <param name="num">OtherSceneの配列番号</param>
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
        /// プレイシーンを読み込む際の切り替えるシーンの要素番
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public SceneManager SetPlayDefaultScene(int num)
        {
            m_PlaySceneDefaultNum = num;
            return this;
        }
        /// <summary>
        /// シーンを任意のOtherScene以外を読み込む
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
        /// サブシーンを表示する
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
        /// サブシーンの読み込みを非同期で行う
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private IEnumerator LoadSubScene(int num)
        {
            // サブシーンを非同期でロード
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_SceneData.SubScene[num], LoadSceneMode.Additive);

            // ロードが完了するまで待機
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // サブシーンを取得
            m_SubScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(m_SceneData.SubScene[num]);

            //イベントシステムの現在のシーンの選択しているオブジェクトを無効にする
            EventSystem.current.enabled = false;

            // サブシーンをアクティブにする
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(m_SubScene);
        }

        /// <summary>
        /// フェードするかを決める
        /// </summary>
        /// <param name="fade">フェードをするかどうか</param>
        /// <param name="unScale">TimeScaleが0でもフェードするかどうか</param>
        /// <returns></returns>
        public SceneManager SetFade(bool fade, bool unScale = true)
        {
            isFade = fade;
            IsUnscale = unScale;
            return this;
        }


        /// <summary>
        /// シーンが読み込まれたら呼ばれる
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
