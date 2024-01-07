using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPositionNode : ActionNode
{
    public float speed = 5;                 //移動速度
    public float stoppingDistance = 0.1f;   //止まる距離
    public bool updateRotation = true;      //回転させるかどうか
    public float acceleration = 40.0f;      //速度加速
    public float tolerance = 1.0f;          //許容範囲

    public bool isDash = false;

    protected override void OnStart()
    {
        //ナビメッシュの設定
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

        //経路探索の準備ができているかどうか
        if (context.agent.pathPending)
        {
            return State.Running;
        }

        //ゴールまでの残り距離
        if (context.agent.remainingDistance < tolerance)
        {
            return State.Success;
        }

        //指定した場所にパスがない場合
        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }
}