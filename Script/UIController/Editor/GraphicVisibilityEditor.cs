using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphicVisibilityController))]
public class GraphicVisibilityEditor : Editor
{
    private GraphicVisibilityController m_Target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        m_Target = target as GraphicVisibilityController;

        if (m_Target.IsSelectObjectOnActivate)
        {
            m_Target.SelectObject = EditorGUILayout.ObjectField(m_Target.SelectObject, typeof(GameObject), true) as GameObject;
        }
    }
}
