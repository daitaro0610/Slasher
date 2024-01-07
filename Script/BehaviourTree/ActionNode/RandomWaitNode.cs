using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWaitNode : ActionNode
{
    [System.Serializable]
    public struct RandomDuration
    {
        public float Min;
        public float Max;
    }
    [SerializeField]
    private RandomDuration m_RandomDuration;

    float m_Duration;
    float m_StartTime;
    protected override void OnStart()
    {
        m_StartTime = Time.time;
        m_Duration = Random.Range(m_RandomDuration.Min, m_RandomDuration.Max);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (Time.time - m_StartTime > m_Duration)
        {
            return State.Success;
        }
        return State.Running;
    }
}