using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetNode : ActionNode
{
    private Vector3 m_TargetPosition;
    [SerializeField]
    private float m_Duration;

    private float m_Time;

    protected override void OnStart()
    {
        m_TargetPosition = context.enemy.GetTarget().transform.position;
        m_Time = 0f;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        m_Time += Time.deltaTime / m_Duration;

        var direction = m_TargetPosition - context.enemy.transform.position;
        direction.y = 0;

        var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        context.enemy.transform.rotation = Quaternion.Slerp(context.enemy.transform.rotation, lookRotation, m_Time);


        if(m_Time >= 1f)
        {
            return State.Success;
        }

        return State.Running;
    }
}