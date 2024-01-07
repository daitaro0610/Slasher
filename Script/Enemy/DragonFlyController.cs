using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class DragonFlyController : EnemyControllerBase
{
    [SerializeField]
    private GameObject m_FlameGeneratePoint;

    private ParticleSystem m_FlameEffect; //エフェクト発生時にそのデータを持っておく
    [SerializeField]
    private Vector3 m_FlameEffectRotation; //ドラゴンブレスの回転の補正

    private GeyserController m_GeyserController;
    private ColorGradingController m_ColorGradingController;

    public override void Attack()
    {
        m_Animation.Attack(0);
        m_Attack.SetDamageValue(0);
    }

    public override void StrongAttack()
    {
        //強攻撃
        m_Animation.Attack(1);
        m_Attack.SetDamageValue(1);
    }

    #region アニメーションイベント用のコールバック処理

    private void AttackAnim(int colliderIndex)
     => m_Attack.SetCollider(colliderIndex).Attack();

    public void UpdateAttackAnim(int colliderIndex)
    {
        m_Attack.UpdateAttackEnabled(true);
        m_Attack.SetCollider(colliderIndex).SetDamageValue(1);
    }

    public void EndAttackAnim()
        => m_Attack.UpdateAttackEnabled(false);


    private void FlameStartAnim(int index)
    {
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.Breath,gameObject);
        m_FlameEffect = EffectManager.instance.Play(EffectName.Breath, m_FlameGeneratePoint, rotation: m_FlameEffectRotation);
    }
    private void FlameEndAnim()
    {
        //m_Attack.UpdateAttackEnabled(false);
        m_FlameEffect.Stop();
    }

    protected override void Dizzy()
    {
        base.Dizzy();
        if(m_FlameEffect != null && m_FlameEffect.isPlaying)
        m_FlameEffect.Stop();
    }

    private void RoarAnim()
    {
        m_GeyserController = FindAnyObjectByType<GeyserController>();
        //咆哮したタイミングで間欠泉をすべて作動させる
        m_GeyserController.AllGeyserPlay();

        m_ColorGradingController = FindAnyObjectByType<ColorGradingController>();
        //咆哮したタイミングで画面を赤めにする(かっこよさのため)
        m_ColorGradingController.ApplyRedTintUsingChannelMixer();

        MusicController.Instance.PlayAngryBossBgm();

        AudioManager.instance.PlaySEFromObjectPosition(SoundName.BossRoar,gameObject);
    }

    private void FlappingAnim()
    {
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.Flapping,gameObject);
    }

    #endregion

    public override void SpecialAttack()
    {
        m_Animation.Flame();
    }

    public void Fly()
    {
        m_Animation.Fly();
    }

    public void Land()
    {
        m_Animation.Land();
    }

    public override void Damage()
    {
        //プレイヤーを見つけていない場合に一度だけ呼ばれる処理
        if (!m_IsPlayerFound)
        {
            m_TargetObject = GameObject.FindGameObjectWithTag(TagName.Player);
            m_IsPlayerFound = true;

            MusicController.Instance.PlayBossBgm();
        }
    }

    public override void Angry()
    {
        m_IsAngry = true;
    }

    public override void Death()
    {
        m_Animation.Death();
        var behaviourRunner = GetComponent<BehaviourTreeRunner>();
        behaviourRunner.SetActive(false);

        //エフェクトを出している途中だった場合は削除
        if (m_FlameEffect != null)
            m_FlameEffect.Stop();

        //敵のコライダーを非表示にする
        m_ColliderDisabler.ColliderDisable();

        //最後にベースの処理を行う
        base.Death();

        GameManager.Instance.MonsterDeath();
    }

    private void OnTriggerStay(Collider other)
    {
        //プレイヤーを見つけていない場合は探す
        if (!m_IsPlayerFound)
        {
            if (other.CompareTag(TagName.Player))
            {
                PlayerInRange(other);
            }
        }
    }
}
