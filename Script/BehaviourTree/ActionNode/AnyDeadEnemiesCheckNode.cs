using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyDeadEnemiesCheckNode : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.enemy.AnyDeadEnemies())
        {
            return State.Success;
        }


        return State.Running;
    }
}