using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTargetPosition : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.agent.SetDestination(context.enemy.GetTarget().transform.position);
        return State.Running;
    }
}