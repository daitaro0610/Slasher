using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyContext
{
    public GameObject gameObject;
    public Transform transform;
    public Rigidbody physics;
    public EnemyControllerBase enemy;
    public NavMeshAgent agent;

    public static EnemyContext CreateFromGameObject(GameObject gameObject)
    {
        EnemyContext context = new();
        context.gameObject = gameObject;
        context.transform = gameObject.transform;
        context.physics = gameObject.GetComponent<Rigidbody>();
        context.enemy = gameObject.GetComponent<EnemyControllerBase>();
        context.agent = gameObject.GetComponent<NavMeshAgent>();

        return context;
    }
}
