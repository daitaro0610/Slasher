using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    private PlayerController m_Player;

    [SerializeField, Header("軽減率")]
    private int m_Armor = 100;

    [SerializeField, Header("ダメージをうけたときの無敵時間")]
    private float m_DamageInvincibilityDuration = 1.0f;
    private float m_DamageInvincibleTimer;//無敵時間の測定用

    private bool m_IsInvincible = false;

    [SerializeField, Header("体力を事前に保存した値に上書きする場合はTrue")]
    private bool m_IsHealthOverride = false;

    private const string HEALTH_DATA_NAME = "HealthData";

    public override void Start()
    {
        base.Start();
        m_Player = GetComponent<PlayerController>();
        if (m_IsHealthOverride)
        {
            //上書きする場合は読み込み処理を行う
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

        //回避状態の場合はダメージをうけないようにする
        if (m_IsInvincible) return;

        if (m_Player.GetState() == PlayerController.State.Avoidance) return;

        //カウンターの場合はカウンター用の処理を行う
        if (m_Player.GetState() == PlayerController.State.Counter)
        {
            Debugger.Log("カウンター");
            m_Player.Counter();
            m_IsInvincible = true;
            return;
        }

        damageValue = DamageCalculator.Calculate(m_Armor, damageValue);
        base.ReceiveDamage(damageValue);

        m_IsInvincible = true;

        if (m_Health <= 0)
        {
            //死亡した場合は初期値に戻す
            PlayerPrefs.SetInt(HEALTH_DATA_NAME, m_MaxHealth);
            //死亡時の処理
            m_Health = 0;
            m_State = State.Death;
            m_Player.Death();
        }
        else
        {
            //値を保存する
            PlayerPrefs.SetInt(HEALTH_DATA_NAME, m_Health);
            m_Player.Damage();
        }
    }

    /// <summary>
    /// HPを回復する
    /// </summary>
    /// <param name="value"></param>
    public void Heal(int value)
    {
        m_Health += value;
        m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);

        PlayerPrefs.SetInt(HEALTH_DATA_NAME, m_Health);
    }
}
