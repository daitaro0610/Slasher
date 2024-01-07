using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderDisabler : MonoBehaviour
{
    [SerializeField]
    private Collider[] m_EnemyColliders;

    public void ColliderDisable()
    {
        foreach(Collider collider in m_EnemyColliders)
        {
            collider.enabled = false;
        }
    }
}
