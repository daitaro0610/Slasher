using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPositionNode : ActionNode
{
    public float speed = 5;                 //�ړ����x
    public float stoppingDistance = 0.1f;   //�~�܂鋗��
    public bool updateRotation = true;      //��]�����邩�ǂ���
    public float acceleration = 40.0f;      //���x����
    public float tolerance = 1.0f;          //���e�͈�

    public bool isDash = false;

    protected override void OnStart()
    {
        //�i�r���b�V���̐ݒ�
        context.agent.SetDestination(blackboard.m_MoveToPosition);
        context.agent.speed = speed;
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
        context.agent.isStopped = false;

        if (!isDash) context.enemy.GetAnimation().SetMove(1);
        else  context.enemy.GetAnimation().SetMove(2);
    }

    protected override void OnStop()
    {
        context.enemy.GetAnimation().SetMove(0);
    }

    protected override State OnUpdate()
    {

        //�o�H�T���̏������ł��Ă��邩�ǂ���
        if (context.agent.pathPending)
        {
            return State.Running;
        }

        //�S�[���܂ł̎c�苗��
        if (context.agent.remainingDistance < tolerance)
        {
            return State.Success;
        }

        //�w�肵���ꏊ�Ƀp�X���Ȃ��ꍇ
        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }
}