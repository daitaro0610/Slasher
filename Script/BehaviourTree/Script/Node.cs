using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }
    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public EnemyContext context;
    [HideInInspector] public Blackboard blackboard;
    [TextArea] public string description;

    public State Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;


    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    public void Abort()
    {
        BehaviourTree.Traverse(this, (node) =>
        {
            node.started = false;
            node.state = State.Running;
            node.OnStop();
        });
    }

    /// <summary>
    /// 開始時に呼ばれる処理
    /// </summary>
    protected abstract void OnStart();
    /// <summary>
    /// 終了時に呼ばれる処理
    /// </summary>
    protected abstract void OnStop();
    /// <summary>
    /// 自身のノードがアクティブ中は常に呼ばれる
    /// </summary>
    /// <returns></returns>
    protected abstract State OnUpdate();
}
