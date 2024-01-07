using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetPosition : ActionNode
{
    protected override void OnStart() {
        blackboard.m_MoveToPosition = context.enemy.GetTarget().transform.position;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}