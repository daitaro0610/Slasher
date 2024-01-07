using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerachPlayerNode : ActionNode
{
    
    protected override void OnStart() {
    }

    protected override void OnStop() {
        
    }

    protected override State OnUpdate() {

        //プレイヤーを探し、見つけるまで続ける
        if (context.enemy.SearchPlayer())
        {
            return State.Success;
        }

        return State.Running;
    }
}