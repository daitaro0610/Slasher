using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class RebindSaveSystem
{
    private const string SAVE_PATH = "ActionAssetData.json";

    public void Save(InputActionAsset inputActionAsset)
    {
        if (inputActionAsset == null) return;

        string json = inputActionAsset.SaveBindingOverridesAsJson();

        string path = Path.Combine(Application.persistentDataPath , SAVE_PATH);

        //�f�[�^���e�L�X�g�ɕۑ�����
        File.WriteAllText(path, json);
    }

    public void Load(InputActionAsset inputActionAsset)
    {
        if (inputActionAsset == null) return;

        string path = Path.Combine(Application.persistentDataPath, SAVE_PATH);
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        //Json�f�[�^��ǂݍ���
        inputActionAsset.LoadBindingOverridesFromJson(json);
    }
}
