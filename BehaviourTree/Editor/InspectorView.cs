using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor m_Editor;

    public InspectorView()
    {

    }

    internal void UpdateSelection(NodeView nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(m_Editor);

        m_Editor = Editor.CreateEditor(nodeView.m_Node);
        IMGUIContainer container = new(() =>
        {
            if (m_Editor.target)
            {
                m_Editor.OnInspectorGUI();
            }
        });
        Add(container);
    }

    internal void UpdateSelection(NodeCommentView commentView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(m_Editor);

        m_Editor = Editor.CreateEditor(commentView.m_Comment);
        IMGUIContainer container = new(() =>
        {
            if (m_Editor.target)
            {
                m_Editor.OnInspectorGUI();

                if (commentView.m_Comment.size != commentView.m_Comment.currentSize)
                {
                    commentView.m_Comment.NotifiData();
                }
            }
        });
        Add(container);
    }
}
