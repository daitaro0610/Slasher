using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    int m_Current;
    protected override void OnStart()
    {
        m_Current = 0;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        var child = m_Children[m_Current];
        switch (child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
            case State.Success:
                m_Current++;
                break;
        }

        return m_Current == m_Children.Count ? State.Success : State.Running;
    }
}
