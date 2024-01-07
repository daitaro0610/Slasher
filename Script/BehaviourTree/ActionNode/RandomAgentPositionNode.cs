using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAgentPositionNode : ActionNode
{
    [System.Serializable]
    public struct RandomArea
    {
        public Vector2 MinPosition;
        public Vector2 MaxPosition;
    }
    [SerializeField]
    private RandomArea m_Area;

    protected override void OnStart() {

        var position = SetRandomValue();
        blackboard.m_MoveToPosition = position;

        context.agent.isStopped = true;
    }

    private Vector3 SetRandomValue()
    {
        var posX = Random.Range(m_Area.MinPosition.x, m_Area.MaxPosition.x);
        var posZ = Random.Range(m_Area.MinPosition.y, m_Area.MaxPosition.y);

        return new Vector3(posX, context.gameObject.transform.position.y, posZ);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}