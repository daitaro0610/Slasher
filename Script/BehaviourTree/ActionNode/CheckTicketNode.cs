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
        //Ticket���g�p�o�����ꍇ�́ASuccess�ɂ���B�����łȂ��ꍇ��Ticket���g�p�ł���܂őҋ@����
        return TicketedAttackManager.Instance.UseTicket() ? State.Success : State.Running;
    }
}