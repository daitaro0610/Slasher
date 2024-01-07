using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetRuntimeNode : ActionNode
{
    [SerializeField]
    private float m_Duration = 3f;

    [SerializeField,Range(0,1)]
    private float m_RotationSpeed = 0.1f;

    private float m_Timer;

    protected override void OnStart()
    {
        m_Timer = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {

        m_Timer += Time.deltaTime;
        if (m_Timer > m_Duration)
        {
            return State.Success;
        }

        var direction = context.enemy.GetTarget().transform.position - context.enemy.transform.position;
        direction.y = 0;

        var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        context.enemy.transform.rotation = Quaternion.Lerp(context.enemy.transform.rotation, lookRotation,m_RotationSpeed);

        return State.Running;
    }
}