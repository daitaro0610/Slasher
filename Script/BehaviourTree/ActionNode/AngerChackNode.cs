using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngerChackNode : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return context.enemy.CheckAngry() ? State.Success : State.Running;
    }
}