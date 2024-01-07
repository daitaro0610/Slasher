using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class GameManager : MonoBehaviour
{
    //シングルトン
    private static GameManager m_Instance;
    public static GameManager Instance => m_Instance;

    [SerializeField]
    private int m_SceneIndex;

    private float m_StageTimer;

    [SerializeField]
    private ClearEffectController m_ClearEffectController; //クリア時の演出クラス
    [SerializeField]
    private float m_ClearEffectWaitTime = 15f;

    [SerializeField]
    private ResultTimerTextController m_ResultTimerTextController; //ステージごとの時間を計測し、ランク決めをするクラス

    [SerializeField]
    private GameOverController m_GameOverController;

    [SerializeField]
    private float m_SceneChangeWaitTime = 5f;　//シーンが切り替わるまでの時間

    [SerializeField]
    private StageTextDialogue m_StageTextDialogue;
    [SerializeField]
    private float m_WaitTime;
    [SerializeField]
    private GraphicVisibilityController m_StageTextGraphic;

    private bool m_IsTextDisplayFinished;
    public bool IsTextDisplayFinished => m_IsTextDisplayFinished;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            m_StageTimer = 0;
            m_IsTextDisplayFinished = false;
        }
        else Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        new MusicController();

        yield return new WaitUntil(() => m_StageTextDialogue.IsCompleteText);

        //指定秒待機した後、画面を明るくしていく
        yield return new WaitForSeconds(m_WaitTime);
        m_StageTextGraphic.SetFadeActive(false);
        //音を徐々に鳴らす
        MusicController.Instance.PlayStageBgm();

        yield return new WaitForSeconds(m_WaitTime);
        //テキストの表示や画面の表示が終わったらTrueにする
        m_IsTextDisplayFinished = true;

    }

    private void Update()
    {
        //経過時間をスコアとする
        m_StageTimer += Time.deltaTime;
    }

    public void MonsterDeath()
    {
        //スコアを保存する
        new ScoreManager(m_SceneIndex, m_StageTimer);

        //ボス用のカメラが存在している場合はカメラを切り替える
        var bossCameraObj = GameObject.FindGameObjectWithTag(TagName.BossCamera);


        if (bossCameraObj != null)
        {
            var bossCamera = bossCameraObj.GetComponent<BossEnemyHuntingController>();
            bossCamera.BossHunted();

            //クリア時のBGMを鳴らす
            MusicController.Instance.PlayClearBgm();

            //指定時間秒待機する
            StartCoroutine(ClearWait());
        }
        else
        {
            MusicController.Instance.StopFadeBgm();

            StartCoroutine(MovieWait());
            return;
        }

    }

    public void PlayerDeath()
    {
        m_GameOverController.GameOver();
    }

    private IEnumerator ClearWait()
    {
        yield return new WaitForSeconds(m_ClearEffectWaitTime);
        //クリア演出
        m_ClearEffectController.OnClear();

        //結果を表示する
        m_ResultTimerTextController.ResultDisplay();

        yield return new WaitForSeconds(15);
        OnTitleTrandision();
    }

    private IEnumerator MovieWait()
    {
        //ボスのムービーが終わるまで待機する
        yield return new WaitWhile(() => BossEnemyHuntingController.IsMovie);

        yield return new WaitForSeconds(m_SceneChangeWaitTime);

        SceneManager.instance.NextScene();
    }

    public void OnTitleTrandision()
    {
        MusicController.Instance.StopFadeBgm();
        SceneManager.instance.TitleLoad();
    }
}
