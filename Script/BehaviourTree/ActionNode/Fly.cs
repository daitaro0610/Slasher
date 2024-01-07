using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : ActionNode
{
    protected override void OnStart() {
        if(context.enemy is DragonFlyController flyEnemy)
        {
            flyEnemy.Fly();
        }
        else
        {
            Debugger.Log("DragonFlyController��������܂���");
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}