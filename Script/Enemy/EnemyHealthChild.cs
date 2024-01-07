using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthChild : MonoBehaviour, IDamageble
{
    private EnemyHealth m_Base;

    [SerializeField,Header("åyå∏ó¶")]
    private int m_Armor = 100;

    private void Awake()
    {
        m_Base = transform.root.gameObject.GetComponent<EnemyHealth>();
    }



    public void ReceiveDamage(int damageValue)
    {
        damageValue = DamageCalculator.Calculate(m_Armor,damageValue);
        if (m_Base.ReceiveDamages(damageValue))
        {
            DamageEffectManager.Instance.InstantiateDamageText(damageValue,transform.position);
        }
    }
}
