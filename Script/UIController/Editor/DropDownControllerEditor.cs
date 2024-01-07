using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DropDownController))]
public class DropDownControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Create Content"))
        {
            DropDownController dropDown = target as DropDownController;
            dropDown.CreateContent();
        }
    }
}
