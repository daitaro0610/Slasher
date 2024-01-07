using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneData))]//拡張するクラスを指定
public class SceneDataButton :Editor
{
    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI();

        if (GUILayout.Button("保存"))
        {
            SceneData sceneData = target as SceneData;
            sceneData.SaveName();
        }
    }
}
