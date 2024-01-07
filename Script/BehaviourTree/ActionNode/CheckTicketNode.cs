using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTicketNode : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //Ticketを使用出来た場合は、Successにする。そうでない場合はTicketを使用できるまで待機する
        return TicketedAttackManager.Instance.UseTicket() ? State.Success : State.Running;
    }
}