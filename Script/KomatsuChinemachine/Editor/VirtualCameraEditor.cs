using UnityEngine;
using UnityEditor;
using KD.Cinemachine;

[CustomEditor(typeof(VirtualCamera))]
public class VirtualCameraEditor : Editor
{

    SerializedProperty m_VirtualCameraSettings;
    private void OnEnable()
    {
        m_VirtualCameraSettings = serializedObject.FindProperty("m_Settings");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VirtualCamera virtualCamera = target as VirtualCamera;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(m_VirtualCameraSettings, new GUIContent("VirtualSettings"));
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Fix", GUILayout.Width(30)))
        {
            virtualCamera.SetParameter();
        }

        EditorGUILayout.EndHorizontal();


        if (virtualCamera.gameObject.GetComponent<VirtualCameraCollider>() == null && GUILayout.Button("CreateVirtualCollider"))
        {
            virtualCamera.gameObject.AddComponent<VirtualCameraCollider>();
        }

    }
}
