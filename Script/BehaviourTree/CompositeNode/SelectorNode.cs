using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    public int m_Current;

    protected override void OnStart() {
        m_Current = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {

        //子ノードの中から、ひとつ選択する
        for(int i = m_Current; i < m_Children.Count; i++)
        {
            m_Current = i;
            var child = m_Children[m_Current];

            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Success:
                    return State.Success;
                case State.Failure:
                    continue;
            }
        }

        return State.Failure;
    }
}