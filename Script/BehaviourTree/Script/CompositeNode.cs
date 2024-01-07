using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> m_Children = new();

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.m_Children = m_Children.ConvertAll(c => c.Clone());
        return node;
    }
}
