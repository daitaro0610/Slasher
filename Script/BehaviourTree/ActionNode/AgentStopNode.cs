using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStopNode : ActionNode
{
    protected override void OnStart() {
        context.agent.isStopped = true;
        context.enemy.GetAnimation().SetMove(0);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}