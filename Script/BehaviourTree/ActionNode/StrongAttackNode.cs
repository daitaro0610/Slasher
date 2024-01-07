using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAttackNode : ActionNode
{
    protected override void OnStart() {
        context.enemy.StrongAttack();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}