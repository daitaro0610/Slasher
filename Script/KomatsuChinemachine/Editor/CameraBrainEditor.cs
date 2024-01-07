using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KD.Cinemachine;

[CustomEditor(typeof(CameraBrain))]
public class CameraBrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var cameraBrain = (CameraBrain)target;

        if (GUILayout.Button("CreateVirtualCamera"))
        {
            cameraBrain.CreateVirtualCamera();
        }
    }
}
