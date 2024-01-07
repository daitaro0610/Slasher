using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDeathNode : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return context.enemy.CheckDeath() ? State.Success : State.Running;
    }
}