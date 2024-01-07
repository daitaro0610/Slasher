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

        //データをテキストに保存する
        File.WriteAllText(path, json);
    }

    public void Load(InputActionAsset inputActionAsset)
    {
        if (inputActionAsset == null) return;

        string path = Path.Combine(Application.persistentDataPath, SAVE_PATH);
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        //Jsonデータを読み込む
        inputActionAsset.LoadBindingOverridesFromJson(json);
    }
}
