using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarNode : ActionNode
{

    private enum AngrySate
    {
        Normal,
        Angry
    }
    [SerializeField]
    private AngrySate m_State;

    protected override void OnStart() {
        switch (m_State)
        {
            case AngrySate.Normal:
                context.enemy.Roar();
                break;
            case AngrySate.Angry:
                context.enemy.AngryRoar();
                break;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}