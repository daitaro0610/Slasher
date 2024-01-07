using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 子ノードをすべて同時に実行する
/// </summary>
public class Parallel : CompositeNode
{
    [SerializeField]
    List<State> m_ChildrenLeftToExecute = new();


    protected override void OnStart() {
        m_ChildrenLeftToExecute.Clear();
        m_Children.ForEach(a =>
        {
            m_ChildrenLeftToExecute.Add(State.Running);
        });
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        bool stillRunning = false;
        for(int i = 0; i < m_ChildrenLeftToExecute.Count(); ++i)
        {
            if(m_ChildrenLeftToExecute[i] == State.Running)
            {
                var status = m_Children[i].Update();
                if(status == State.Failure)
                {
                    AbordRunningChildren();
                    return State.Failure;
                }

                if(status == State.Running)
                {
                    stillRunning = true;
                }

                m_ChildrenLeftToExecute[i] = status;
            }
        }

        return stillRunning ? State.Running : State.Success;
    }

    void AbordRunningChildren()
    {
        for(int i = 0; i < m_ChildrenLeftToExecute.Count(); ++i)
        {
            if(m_ChildrenLeftToExecute[i] == State.Running)
            {
                m_Children[i].Abort();
            }
        }
    }
}