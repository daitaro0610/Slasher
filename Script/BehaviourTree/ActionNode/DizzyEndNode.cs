using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DizzyEndNode : ActionNode
{
    protected override void OnStart() {
        context.enemy.DizzyEnd();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}