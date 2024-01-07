using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KD.Cinemachine;

[CustomEditor(typeof(VirtualCameraCollider))]
public class VirutalCameraColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        {
            VirtualCameraCollider VCC = (VirtualCameraCollider)target;
            VCC.m_IgnoreTag = EditorGUILayout.TagField("Ignore Tag", VCC.m_IgnoreTag);

            if (GUILayout.Button("Clear",GUILayout.Width(60)))
            {
                VCC.TagClear();
            }
        }
        GUILayout.EndHorizontal();
    }
}
