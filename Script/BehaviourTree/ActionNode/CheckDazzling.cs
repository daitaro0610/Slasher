using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDazzling : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return context.enemy.CheckDazzling()? State.Success : State.Running;
    }
}