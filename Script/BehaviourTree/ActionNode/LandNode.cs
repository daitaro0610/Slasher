using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandNode : ActionNode
{
    protected override void OnStart() {
        if (context.enemy is DragonFlyController flyEnemy)
        {
            flyEnemy.Land();
        }
        else
        {
            Debugger.Log("DragonFlyController‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}