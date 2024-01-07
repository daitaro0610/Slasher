using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD;

public class DragonFlyController : EnemyControllerBase
{
    [SerializeField]
    private GameObject m_FlameGeneratePoint;

    private ParticleSystem m_FlameEffect; //�G�t�F�N�g�������ɂ��̃f�[�^�������Ă���
    [SerializeField]
    private Vector3 m_FlameEffectRotation; //�h���S���u���X�̉�]�̕␳

    private GeyserController m_GeyserController;
    private ColorGradingController m_ColorGradingController;

    public override void Attack()
    {
        m_Animation.Attack(0);
        m_Attack.SetDamageValue(0);
    }

    public override void StrongAttack()
    {
        //���U��
        m_Animation.Attack(1);
        m_Attack.SetDamageValue(1);
    }

    #region �A�j���[�V�����C�x���g�p�̃R�[���o�b�N����

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
        //���K�����^�C�~���O�ŊԌ�������ׂč쓮������
        m_GeyserController.AllGeyserPlay();

        m_ColorGradingController = FindAnyObjectByType<ColorGradingController>();
        //���K�����^�C�~���O�ŉ�ʂ�Ԃ߂ɂ���(�������悳�̂���)
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
        //�v���C���[�������Ă��Ȃ��ꍇ�Ɉ�x�����Ă΂�鏈��
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

        //�G�t�F�N�g���o���Ă���r���������ꍇ�͍폜
        if (m_FlameEffect != null)
            m_FlameEffect.Stop();

        //�G�̃R���C�_�[���\���ɂ���
        m_ColliderDisabler.ColliderDisable();

        //�Ō�Ƀx�[�X�̏������s��
        base.Death();

        GameManager.Instance.MonsterDeath();
    }

    private void OnTriggerStay(Collider other)
    {
        //�v���C���[�������Ă��Ȃ��ꍇ�͒T��
        if (!m_IsPlayerFound)
        {
            if (other.CompareTag(TagName.Player))
            {
                PlayerInRange(other);
            }
        }
    }
}
