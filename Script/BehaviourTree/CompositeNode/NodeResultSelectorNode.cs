using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeResultSelectorNode : CompositeNode
{
    public int m_CheckNodeIndex;

    public int m_SuccessSelectIndex;
    public int m_FailureSelectIndex;

    protected override void OnStart() {
        m_CheckNodeIndex = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {

        switch (m_Children[m_CheckNodeIndex].Update())
        {
            case State.Running:
                m_Children[m_FailureSelectIndex].Update();
                return State.Running;
            case State.Success:
                m_Children[m_SuccessSelectIndex].Update();
                return State.Running;
            case State.Failure:
                return State.Failure;
        }

        return State.Failure;
    }
}