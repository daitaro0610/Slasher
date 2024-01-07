using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class GameManager : MonoBehaviour
{
    //�V���O���g��
    private static GameManager m_Instance;
    public static GameManager Instance => m_Instance;

    [SerializeField]
    private int m_SceneIndex;

    private float m_StageTimer;

    [SerializeField]
    private ClearEffectController m_ClearEffectController; //�N���A���̉��o�N���X
    [SerializeField]
    private float m_ClearEffectWaitTime = 15f;

    [SerializeField]
    private ResultTimerTextController m_ResultTimerTextController; //�X�e�[�W���Ƃ̎��Ԃ��v�����A�����N���߂�����N���X

    [SerializeField]
    private GameOverController m_GameOverController;

    [SerializeField]
    private float m_SceneChangeWaitTime = 5f;�@//�V�[�����؂�ւ��܂ł̎���

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

        //�w��b�ҋ@������A��ʂ𖾂邭���Ă���
        yield return new WaitForSeconds(m_WaitTime);
        m_StageTextGraphic.SetFadeActive(false);
        //�������X�ɖ炷
        MusicController.Instance.PlayStageBgm();

        yield return new WaitForSeconds(m_WaitTime);
        //�e�L�X�g�̕\�����ʂ̕\�����I�������True�ɂ���
        m_IsTextDisplayFinished = true;

    }

    private void Update()
    {
        //�o�ߎ��Ԃ��X�R�A�Ƃ���
        m_StageTimer += Time.deltaTime;
    }

    public void MonsterDeath()
    {
        //�X�R�A��ۑ�����
        new ScoreManager(m_SceneIndex, m_StageTimer);

        //�{�X�p�̃J���������݂��Ă���ꍇ�̓J������؂�ւ���
        var bossCameraObj = GameObject.FindGameObjectWithTag(TagName.BossCamera);


        if (bossCameraObj != null)
        {
            var bossCamera = bossCameraObj.GetComponent<BossEnemyHuntingController>();
            bossCamera.BossHunted();

            //�N���A����BGM��炷
            MusicController.Instance.PlayClearBgm();

            //�w�莞�ԕb�ҋ@����
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
        //�N���A���o
        m_ClearEffectController.OnClear();

        //���ʂ�\������
        m_ResultTimerTextController.ResultDisplay();

        yield return new WaitForSeconds(15);
        OnTitleTrandision();
    }

    private IEnumerator MovieWait()
    {
        //�{�X�̃��[�r�[���I���܂őҋ@����
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
