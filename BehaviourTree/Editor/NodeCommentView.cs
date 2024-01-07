using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class NodeCommentView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeCommentView> OnCommentSelected;
    public NodeComment m_Comment;

    private VisualElement m_Border;

    public NodeCommentView(NodeComment comment) : base("Assets/BehaviourTree/Data/NodeCommentView.uxml")
    {
        this.m_Comment = comment;
        this.name = comment.name;
        this.viewDataKey = comment.guid;

        style.left = comment.position.x;
        style.top = comment.position.y;

        AddToClassList("nodeComment");

        m_Comment.onDataChanged += OnDataChanged;

        //タイトルの表記を変更する
        Label titleLabel = this.Q<Label>("commentTitle");
        titleLabel.bindingPath = "commentTitle";
        titleLabel.Bind(new SerializedObject(comment));

        //コメントの表記を変更する
        Label commentLabel = this.Q<Label>("description");
        commentLabel.bindingPath = "description";
        commentLabel.Bind(new SerializedObject(comment));

        m_Border = this.Q<VisualElement>("comment-border");
        OnDataChanged();
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnCommentSelected != null)
        {
            OnCommentSelected.Invoke(this);
        }
    }

    public override void OnUnselected()
    {
        base.OnUnselected();

        //一番後ろに配置
        this.SendToBack();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(m_Comment, "Behaviour Tree (Set Position)");
        m_Comment.position.x = newPos.xMin;
        m_Comment.position.y = newPos.yMin;
        EditorUtility.SetDirty(m_Comment);
    }

    public void OnDataChanged()
    {
        m_Border.style.width = m_Comment.size.x;
        m_Border.style.height = m_Comment.size.y;

        //一番後ろに配置
        this.SendToBack();
    }
}
