using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private EnemyControllerBase m_Enemy;

    private int m_AngryHealthValue;


    public override void Start()
    {
        base.Start();
        m_AngryHealthValue = m_Health / 2;
        m_Enemy = GetComponent<EnemyControllerBase>();
    }

    public bool ReceiveDamages(int damageValue)
    {
        if (m_State == State.Death)
        {
            return false;
        }
        base.ReceiveDamage(damageValue);

        if(m_Health > 0)
        {
            m_Enemy.Damage();

            if(m_AngryHealthValue >= m_Health)
            {
                m_Enemy.Angry();
            }

        }
        else if (m_State == State.Live)
        {
            m_State = State.Death;
            m_Enemy.Death();
        }

        return true;
    }
}
