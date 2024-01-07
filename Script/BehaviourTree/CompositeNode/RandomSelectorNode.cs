using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelectorNode : CompositeNode
{
    private int m_Current;
    protected override void OnStart() {
        m_Current = Random.Range(0, m_Children.Count);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return m_Children[m_Current].Update();
    }
}