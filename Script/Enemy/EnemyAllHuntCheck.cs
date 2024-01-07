using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAllHuntCheck : MonoBehaviour
{
    private List<EnemyControllerBase> m_DeathEnemy;
    private int m_EnemyCount;

    private void Awake()
    {
        m_DeathEnemy = new();

        m_EnemyCount = GameObject.FindGameObjectsWithTag(TagName.Enemy).Length;
    }

    public void AddDeathEnemy(EnemyControllerBase enemy)
    {
        m_DeathEnemy.Add(enemy);
        if(m_EnemyCount == m_DeathEnemy.Count)
        {
            GameManager.Instance.MonsterDeath();
        }
    }

    public int DeathEnemyCount() => m_DeathEnemy.Count;

   
}
