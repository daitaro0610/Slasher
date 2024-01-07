using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackNode : ActionNode
{
    protected override void OnStart() {
        context.enemy.SpecialAttack();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}