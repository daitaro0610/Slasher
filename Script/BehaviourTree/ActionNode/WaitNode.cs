using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{
    public float m_Duration = 1;
    float m_StartTime;
    protected override void OnStart()
    {
        m_StartTime = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if(Time.time - m_StartTime > m_Duration)
        {
            return State.Success;
        }
        return State.Running;
    }
}
