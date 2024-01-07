using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTicketNode : ActionNode
{
    protected override void OnStart() {
        TicketedAttackManager.Instance.ReturnTicket();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}