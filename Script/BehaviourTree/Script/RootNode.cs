using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    public Node m_Child;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return m_Child.Update();
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.m_Child = m_Child.Clone();
        return node;
    }
}
