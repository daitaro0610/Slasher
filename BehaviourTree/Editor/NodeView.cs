using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node m_Node;
    public Port m_Input;
    public Port m_Output;
    public NodeView(Node node) : base("Assets/BehaviourTree/Data/NodeView.uxml")
    {
        this.m_Node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (m_Node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (m_Node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if (m_Node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (m_Node is RootNode)
        {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts()
    {
        if (m_Node is ActionNode)
        {
            m_Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (m_Node is CompositeNode)
        {
            m_Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (m_Node is DecoratorNode)
        {
            m_Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (m_Node is RootNode)
        {
        }

        if (m_Input != null)
        {
            m_Input.portName = "";
            m_Input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(m_Input);
        }
    }

    private void CreateOutputPorts()
    {
        if (m_Node is ActionNode)
        {

        }
        else if (m_Node is CompositeNode)
        {
            m_Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (m_Node is DecoratorNode)
        {
            m_Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (m_Node is RootNode)
        {
            m_Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (m_Output != null)
        {
            m_Output.portName = "";
            m_Output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(m_Output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(m_Node, "Behaviour Tree (Set Position)");
        m_Node.position.x = newPos.xMin;
        m_Node.position.y = newPos.yMin;
        EditorUtility.SetDirty(m_Node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public void SortChildren()
    {
        CompositeNode composite = m_Node as CompositeNode;
        if (composite)
        {
            composite.m_Children.Sort(SortByHorizontalPosition);
        }
    }

    /// <summary>
    /// ノードの位置に応じてリストの順番を変更する
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (m_Node.state)
            {
                case Node.State.Running:
                    if (m_Node.started)
                    {
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
