using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFlameController : MonoBehaviour
{
    [SerializeField]
    private int m_DamageValue;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(TagName.Player))
        {
            IDamageble damageble = other.GetComponent<PlayerHealth>();
            int damage = AttackPowerCalculator.Attack(m_DamageValue);
            damageble.ReceiveDamage(damage);
        }
    }
}
