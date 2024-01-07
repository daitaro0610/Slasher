using UnityEngine;
using KD;

public class DragonController : EnemyControllerBase
{
    public override void Angry()
    {
        m_IsAngry = true;
    }

    public override void SpecialAttack()
    {
        m_Animation.Flame();
    }

    public override void Attack()
    {
        m_Animation.Attack(0);
        m_Attack.SetDamageValue(0);
    }

    public override void StrongAttack()
    {
        m_Animation.Attack(1);
        m_Attack.SetDamageValue(1);
    }

    public override void Damage()
    {
        if (!m_IsPlayerFound)
        {
            MusicController.Instance.PlayBattleBgm();
            var enemies = FindObjectsByType<DragonController>(FindObjectsSortMode.InstanceID);
            foreach (var enemy in enemies)
            {
                enemy.TrackingTarget();
            }
        }
    }

    public void TrackingTarget()
    {
        m_TargetObject = GameObject.FindGameObjectWithTag(TagName.Player);
        m_IsPlayerFound = true;
    }

    public override void Death()
    {
        m_Animation.Death();
        var behaviourRunner = GetComponent<BehaviourTreeRunner>();
        behaviourRunner.SetActive(false);

        m_ColliderDisabler.ColliderDisable();

        base.Death();

        TicketedAttackManager.Instance.ReturnTicket();
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
        m_Attack.UpdateAttackEnabled(true);
        m_Attack.SetCollider(index).SetDamageValue(2);
    }
    private void FlameEndAnim()
        => m_Attack.UpdateAttackEnabled(false);

    private void ZakoRoarAnim()
    {
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.ZakoRoar,gameObject);
    }

    private void StrongRoarAnim()
    {
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.MidBossRoar,gameObject);
    }

    #endregion

    private void OnTriggerEnter(Collider other)
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
