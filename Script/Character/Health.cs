using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour,IDamageble
{
    [SerializeField,Header("‘Ì—Í")]
    protected int m_Health = 100;
    protected int m_MaxHealth;

    public int GetMaxHealth() => m_MaxHealth;

    public int GetHealth() => m_Health;


    public enum State
    {
        Live,
        Death,
    }
    protected State m_State;

    public State GetState() => m_State;

    public virtual void Start()
    {
        m_State = State.Live;
        m_MaxHealth = m_Health;
    }

    public virtual void ReceiveDamage(int damageValue)
    {
        if (m_State == State.Death) return;
        m_Health -= damageValue;
    }
}
