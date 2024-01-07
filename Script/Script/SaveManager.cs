using System.Collections.Generic;
using UnityEngine;
using System.IO;

public interface ISave { }

namespace KD
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance;

        [SerializeField]
        private string path = "/SaveData/";

        private readonly string FilePath = "/#NAME#.json";

        private void Awake()
        {
            //インスタンス化する
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                //リストの初期化
                if(!Directory.Exists(Application.dataPath + path)){
                    Debugger.Log("保存先のフォルダが見つからなかったため新規作成しました");
                    Directory.CreateDirectory(Application.dataPath + path);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// データを保存する
        /// </summary>
        /// <typeparam name="T">スクリタブルオブジェクトの値を保存する</typeparam>
        /// <param name="data"></param>
        public void Save<T>(T data, string fileName, bool encrypt = false) where T : ISave
        {
            Debugger.Log("セーブする　FileName:" + fileName);
            object o = data;
            string filePath = Application.dataPath + path + FilePath.Replace("#NAME#", fileName);

            string json = JsonUtility.ToJson(o, true);

            //暗号化させる
            if (encrypt)
                json = Encryption.EncryptString(json);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(json);
            //データの書き込み
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// データの読み込みをする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Load<T>(string fileName, T defaultData, bool encrypt = false) where T : ISave
        {
            if (!File.Exists(Application.dataPath + path + FilePath.Replace("#NAME#", fileName)))
            {
                Debugger.Log("データが見つかりません");
                return defaultData;
            }

            StreamReader reader;
            reader = new StreamReader(Application.dataPath + path + FilePath.Replace("#NAME#", fileName));
            string datastr = reader.ReadToEnd();
            reader.Close();

            if (encrypt)
                datastr = Encryption.DecryptString(datastr);

            //デシリアライズする
            JsonUtility.FromJsonOverwrite(datastr, defaultData);
            return defaultData;
        }
    }
}