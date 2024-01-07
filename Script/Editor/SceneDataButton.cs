using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneData))]//�g������N���X���w��
public class SceneDataButton :Editor
{
    /// <summary>
    /// Inspector��GUI���X�V
    /// </summary>
    public override void OnInspectorGUI()
    {
        //����Inspector������\��
        base.OnInspectorGUI();

        if (GUILayout.Button("�ۑ�"))
        {
            SceneData sceneData = target as SceneData;
            sceneData.SaveName();
        }
    }
}
