using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class NodeTemplateSettings : ScriptableObject
{
    public TextAsset m_ActionNodeTemplate;
    public TextAsset m_CompositeNodeTemplate;
    public TextAsset m_DecoratorNodeTemplate;
    public string m_NewNodeBasePath = "Assets/";

    static NodeTemplateSettings FindSettings()
    {
        var guids = AssetDatabase.FindAssets("t:NodeTemplateSettings");
        if (guids.Length > 1)
        {
            Debug.LogWarning($"Found multiple settings files, using the first.");
        }

        switch (guids.Length)
        {
            case 0:
                return null;
            default:
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<NodeTemplateSettings>(path);
        }
    }

    internal static NodeTemplateSettings GetOrCreateSettings()
    {
        var settings = FindSettings();
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<NodeTemplateSettings>();
            AssetDatabase.CreateAsset(settings, "Assets");
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}
