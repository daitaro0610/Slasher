using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    private PlayerController m_Player;

    [SerializeField, Header("�y����")]
    private int m_Armor = 100;

    [SerializeField, Header("�_���[�W���������Ƃ��̖��G����")]
    private float m_DamageInvincibilityDuration = 1.0f;
    private float m_DamageInvincibleTimer;//���G���Ԃ̑���p

    private bool m_IsInvincible = false;

    [SerializeField, Header("�̗͂����O�ɕۑ������l�ɏ㏑������ꍇ��True")]
    private bool m_IsHealthOverride = false;

    private const string HEALTH_DATA_NAME = "HealthData";

    public override void Start()
    {
        base.Start();
        m_Player = GetComponent<PlayerController>();
        if (m_IsHealthOverride)
        {
            //�㏑������ꍇ�͓ǂݍ��ݏ������s��
            m_Health = PlayerPrefs.GetInt(HEALTH_DATA_NAME, m_Health);
        }
    }

    private void Update()
    {
        if (!m_IsInvincible) return;

        m_DamageInvincibleTimer += Time.deltaTime;

        if (m_DamageInvincibilityDuration <= m_DamageInvincibleTimer)
        {
            m_DamageInvincibleTimer = 0;
            m_IsInvincible = false;
        }
    }

    public void SetIsInvincible(bool isbool) => m_IsInvincible = isbool;

    public override void ReceiveDamage(int damageValue)
    {
        if (m_State == State.Death) return;

        //�����Ԃ̏ꍇ�̓_���[�W�������Ȃ��悤�ɂ���
        if (m_IsInvincible) return;

        if (m_Player.GetState() == PlayerController.State.Avoidance) return;

        //�J�E���^�[�̏ꍇ�̓J�E���^�[�p�̏������s��
        if (m_Player.GetState() == PlayerController.State.Counter)
        {
            Debugger.Log("�J�E���^�[");
            m_Player.Counter();
            m_IsInvincible = true;
            return;
        }

        damageValue = DamageCalculator.Calculate(m_Armor, damageValue);
        base.ReceiveDamage(damageValue);

        m_IsInvincible = true;

        if (m_Health <= 0)
        {
            //���S�����ꍇ�͏����l�ɖ߂�
            PlayerPrefs.SetInt(HEALTH_DATA_NAME, m_MaxHealth);
            //���S���̏���
            m_Health = 0;
            m_State = State.Death;
            m_Player.Death();
        }
        else
        {
            //�l��ۑ�����
            PlayerPrefs.SetInt(HEALTH_DATA_NAME, m_Health);
            m_Player.Damage();
        }
    }

    /// <summary>
    /// HP���񕜂���
    /// </summary>
    /// <param name="value"></param>
    public void Heal(int value)
    {
        m_Health += value;
        m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);

        PlayerPrefs.SetInt(HEALTH_DATA_NAME, m_Health);
    }
}
