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
            //�C���X�^���X������
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                //���X�g�̏�����
                if(!Directory.Exists(Application.dataPath + path)){
                    Debugger.Log("�ۑ���̃t�H���_��������Ȃ��������ߐV�K�쐬���܂���");
                    Directory.CreateDirectory(Application.dataPath + path);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// �f�[�^��ۑ�����
        /// </summary>
        /// <typeparam name="T">�X�N���^�u���I�u�W�F�N�g�̒l��ۑ�����</typeparam>
        /// <param name="data"></param>
        public void Save<T>(T data, string fileName, bool encrypt = false) where T : ISave
        {
            Debugger.Log("�Z�[�u����@FileName:" + fileName);
            object o = data;
            string filePath = Application.dataPath + path + FilePath.Replace("#NAME#", fileName);

            string json = JsonUtility.ToJson(o, true);

            //�Í���������
            if (encrypt)
                json = Encryption.EncryptString(json);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(json);
            //�f�[�^�̏�������
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// �f�[�^�̓ǂݍ��݂�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Load<T>(string fileName, T defaultData, bool encrypt = false) where T : ISave
        {
            if (!File.Exists(Application.dataPath + path + FilePath.Replace("#NAME#", fileName)))
            {
                Debugger.Log("�f�[�^��������܂���");
                return defaultData;
            }

            StreamReader reader;
            reader = new StreamReader(Application.dataPath + path + FilePath.Replace("#NAME#", fileName));
            string datastr = reader.ReadToEnd();
            reader.Close();

            if (encrypt)
                datastr = Encryption.DecryptString(datastr);

            //�f�V���A���C�Y����
            JsonUtility.FromJsonOverwrite(datastr, defaultData);
            return defaultData;
        }
    }
}