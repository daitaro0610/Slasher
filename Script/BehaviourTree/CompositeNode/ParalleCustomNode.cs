using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParalleCustomNode : CompositeNode
{
    [SerializeField,ReadOnly]
    List<State> m_ChildrenLeftToExecute = new();

    [Min(0),SerializeField]
    private int m_Current;

    [SerializeField]
    private State m_SuccessState = State.Success;


    protected override void OnStart()
    {
        m_ChildrenLeftToExecute.Clear();
        m_Children.ForEach(a =>
        {
            m_ChildrenLeftToExecute.Add(State.Running);
        });
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        bool stillRunning = false;
        for (int i = 0; i < m_ChildrenLeftToExecute.Count(); ++i)
        {
            if (m_ChildrenLeftToExecute[i] == State.Running)
            {
                var status = m_Children[i].Update();
                if (status == State.Failure)
                {
                    AbordRunningChildren();
                    return State.Failure;
                }

                if(m_Current == i && m_SuccessState == status)
                {
                    stillRunning = true;
                }

                m_ChildrenLeftToExecute[i] = status;
            }
        }

        return stillRunning ?  State.Success : State.Running;
    }

    void AbordRunningChildren()
    {
        for (int i = 0; i < m_ChildrenLeftToExecute.Count(); ++i)
        {
            if (m_ChildrenLeftToExecute[i] == State.Running)
            {
                m_Children[i].Abort();
            }
        }
    }
}