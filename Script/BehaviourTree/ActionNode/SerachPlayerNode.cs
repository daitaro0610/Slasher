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

        //�v���C���[��T���A������܂ő�����
        if (context.enemy.SearchPlayer())
        {
            return State.Success;
        }

        return State.Running;
    }
}