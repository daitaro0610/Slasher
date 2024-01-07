using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using KD.ObjectPool;

namespace KD
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField, ReadOnly]
        private AudioSource m_AudioSourceBgm;

        [SerializeField, ReadOnly]
        private AudioMixer m_Mixer;

        [SerializeField, ReadOnly]
        private AudioDataBase m_AudioDataBase;

        [SerializeField, ReadOnly]
        private AudioMixerGroup m_MixerGroupSe;

        [SerializeField, Tooltip("������SEObject���A�^�b�`����")]
        private GameObject[] m_SePrefabs;//SE�炷�I�u�W�F�N�g�̐���

        [SerializeField, Tooltip("�ő�s�b�`")]
        private float m_MaxPitch = 5.0f;
        [SerializeField, Tooltip("�ŏ��s�b�`")]
        private float m_MinPitch = 0.0f;

        private AudioSource m_VolumeSe;

        [SerializeField, Tooltip("Se�̍ő�̏d����")]
        private int m_MaxSeCount = 30;

        [SerializeField, ReadOnly]
        private int m_SeCount;

        private List<GameObject> m_AudioObjectList;
        private ObjectPool<AudioSource>[] m_Pools;

        //���ʂɂ���Ēl���ω�����
        private float m_FadeSpeed;

        //�R���[�`���p
        private WaitForFixedUpdate waitFixed = new WaitForFixedUpdate();

        private Dictionary<int, Coroutine> m_CheckCoroutineDic = new();

        private int m_CoroutineNumber = 0;

        void Awake()
        {
            // �V���O���g�����A�V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                //������
                m_AudioObjectList = new List<GameObject>();
                m_CheckCoroutineDic = new();
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
                UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnLoaded;

                m_Pools = new ObjectPool<AudioSource>[m_SePrefabs.Length];
                for(int i = 0; i < m_SePrefabs.Length; i++)
                {
                    m_Pools[i] = new(m_SePrefabs[i], transform);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// BGM��炷
        /// </summary>
        /// <param name="name">���̖��O</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        public AudioManager PlayBGM(string name, bool loop = true)
        {
            //���������A�܂���None�Ɠ��͂����ꍇ�͏������Ȃ��悤�ɂ���
            if (name == "" || name == "None") return this;

            m_AudioSourceBgm.loop = loop;
            for (int i = 0; i < m_AudioDataBase.m_BGMData.Length; i++)
            {
                //Type��BGM����Ȃ���Ύ��̃��[�v�ɂ���
                if (m_AudioDataBase.m_BGMData[i].Name == name)
                {
                    if (m_AudioSourceBgm.isPlaying)
                    {
                        m_AudioSourceBgm.Stop();
                    }
                    m_AudioSourceBgm.clip = m_AudioDataBase.m_BGMData[i].AudioClip;
                    m_AudioSourceBgm.Play();
                    break;
                }
            }

            if (!m_AudioSourceBgm.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            return this;
        }

        /// <summary>
        /// �v�f�ԍ����w�肵��BGM��炷
        /// </summary>
        /// <param name="index">AudioData�̗v�f�ԍ�</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        /// <returns></returns>
        public AudioManager PlayBGM(int index, bool loop = true)
        {
            m_AudioSourceBgm.loop = loop;

            if (m_AudioDataBase.m_BGMData.Length <= index)
            {
                Debug.LogError($"�z�񂪒����Ă��܂��@�v�f��:{m_AudioDataBase.m_SEData.Length} index:{index}");
                return this;
            }

            //���ɉ������Ă���ꍇ�͈�x��~���Ă���Đ�����
            if (m_AudioSourceBgm.isPlaying)
            {
                m_AudioSourceBgm.Stop();
            }

            //AudioData���ɂ���BGMData��Clip�������Ė炷�B
            //�Ȃ�Ȃ��ꍇ�̓G���[���o��
            m_AudioSourceBgm.clip = m_AudioDataBase.m_BGMData[index].AudioClip;
            m_AudioSourceBgm.Play();

            if (!m_AudioSourceBgm.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);


            return this;
        }

        /// <summary>
        /// SE��炷
        /// </summary>
        /// <param name="name">SE�̖��O</param>
        /// <param name="seObjectIndex">�ǂ̃v���n�u���g�p���邩</param>
        /// <param name="pitch">���̍����̒���</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        public AudioManager PlaySE(string name, int seObjectIndex = 0,Vector3 position = new Vector3(), float volume = 1f, float pitch = 1, bool loop = false)
        {
            //���������A�܂���None�Ɠ��͂����ꍇ�͏������Ȃ��悤�ɂ���
            if (name == "" || name == "None") return this;

            //����ȏ�AudioSource���������特��炳�Ȃ��悤�ɂ���
            if (m_MaxSeCount < m_SeCount) return this;
            //index�̒l��Length�̒l�𒴂��Ă�����G���[��\������
            if (m_SePrefabs.Length < seObjectIndex)
            {
                Debug.LogError("index�̒l���I�[�o�[���Ă��܂� index:" + seObjectIndex);
                return this;
            }

            //AudioSource�̐����Ɛݒ�
            AudioSource audioSourceSe = m_Pools[seObjectIndex].Get();
            audioSourceSe.transform.position = position;
            audioSourceSe.outputAudioMixerGroup = m_MixerGroupSe;
            audioSourceSe.loop = loop;
            audioSourceSe.volume = volume;

            if (m_MaxPitch < pitch) pitch = m_MaxPitch;
            else if (m_MinPitch > pitch) pitch = m_MinPitch;
            //�s�b�`��ύX����
            audioSourceSe.pitch = pitch;


            for (int i = 0; i < m_AudioDataBase.m_SEData.Length; i++)
            {
                //Type��BGM����Ȃ���Ύ��̃��[�v�ɂ���
                if (m_AudioDataBase.m_SEData[i].Name == name)
                {
                    //Loop�Ȃ�Play
                    if (loop)
                    {
                        audioSourceSe.clip = m_AudioDataBase.m_SEData[i].AudioClip;
                        audioSourceSe.Play();
                    }
                    else audioSourceSe.PlayOneShot(m_AudioDataBase.m_SEData[i].AudioClip);

                    m_SeCount++;
                    break;
                }
            }
            if (!audioSourceSe.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            //�������񂾂�A���g�ɂ��Ă��邱��AudioSource���폜����
            if (!loop)
            {
                m_CoroutineNumber++;
                Coroutine coroutine = StartCoroutine(Checking(audioSourceSe, m_CoroutineNumber, () =>
              {
                  m_Pools[seObjectIndex].Release(audioSourceSe);
                  m_SeCount--;
              }));
                m_CheckCoroutineDic.Add(m_CoroutineNumber, coroutine);
            }

            return this;
        }

        /// <summary>
        /// �v�f�ԍ����w�肵��SE��炷
        /// ��������̂�AudioManager��
        /// </summary>
        /// <param name="index">SE�̗v�f�ԍ�</param>
        /// <param name="seObjectIndex">�ǂ̃v���n�u���g�p���邩</param>
        /// <param name="pitch">���̍�������</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        /// <returns></returns>
        public AudioManager PlaySE(int index, int seObjectIndex = 0, Vector3 position = new Vector3(), float volume = 1f, float pitch = 1, bool loop = false)
        {
            //����ȏ�AudioSource���������特��炳�Ȃ��悤�ɂ���
            if (m_MaxSeCount < m_SeCount) return this;

            //index�̒l��Length�̒l�𒴂��Ă�����G���[��\������
            if (m_SePrefabs.Length < seObjectIndex)
            {
                Debug.LogError("index�̒l���I�[�o�[���Ă��܂� index:" + seObjectIndex);
                return this;
            }

            //AudioSource�̐����Ɛݒ�
            AudioSource audioSourceSe = m_Pools[seObjectIndex].Get();
            audioSourceSe.transform.position = position;
            audioSourceSe.outputAudioMixerGroup = m_MixerGroupSe;
            audioSourceSe.loop = loop;
            audioSourceSe.volume = volume;

            if (m_MaxPitch < pitch) pitch = m_MaxPitch;
            else if (m_MinPitch > pitch) pitch = m_MinPitch;
            //�s�b�`��ύX����
            audioSourceSe.pitch = pitch;

            //�z�񂪒����Ă�����G���[��\������
            if (m_AudioDataBase.m_SEData.Length <= index)
            {
                Debug.LogError($"�z�񂪒����Ă��܂��@�v�f��:{m_AudioDataBase.m_SEData.Length} index:{index}");
                return this;
            }

            //���[�v����ꍇ��Play�A���[�v���Ȃ��ꍇ��OneShot�ɂ���
            if (loop)
            {
                audioSourceSe.clip = m_AudioDataBase.m_SEData[index].AudioClip;
                audioSourceSe.Play();
            }
            else audioSourceSe.PlayOneShot(m_AudioDataBase.m_SEData[index].AudioClip);

            if (!audioSourceSe.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            //�������񂾂�A���g�ɂ��Ă��邱��AudioSource���폜����
            //���񂾍ۂɃR�[���o�b�N���ݒ肳��Ă���ꍇ�͂��̃R�[���o�b�N����������
            if (!loop)
            {
                m_CoroutineNumber++;
                Coroutine coroutine = StartCoroutine(Checking(audioSourceSe, m_CoroutineNumber, () =>
                {
                    m_Pools[seObjectIndex].Release(audioSourceSe);
                    m_SeCount--;
                }));
                m_CheckCoroutineDic.Add(m_CoroutineNumber, coroutine);
            }

            return this;
        }

        /// <summary>
        /// �w�肵���I�u�W�F�N�g����SE��炷
        /// ��������̂̓I�u�W�F�N�g��
        /// </summary>
        /// <param name="name">SE�̖��O</param>
        /// <param name="parentObj">�e�ƂȂ�I�u�W�F�N�g</param>
        /// <param name="offset">�I�u�W�F�N�g�𐶐�����ʒu�̒���</param>
        /// <param name="seObjectIndex">�ǂ̃v���n�u���g�p���邩</param>
        /// <param name="pitch">���̍����̒���</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        public AudioManager PlaySEAsChild(string name, Transform parent, Vector3? offset = null, int seObjectIndex = 0, float volume = 1f, float pitch = 9999, bool loop = false)
        {
            //���������A�܂���None�Ɠ��͂����ꍇ�͏������Ȃ��悤�ɂ���
            if (name == "" || name == "None") return this;

            //����ȏ�AudioSource���������特��炳�Ȃ��悤�ɂ���
            if (m_MaxSeCount < m_SeCount) return this;
            //index�̒l��Length�̒l�𒴂��Ă�����G���[��\������
            if (m_SePrefabs.Length < seObjectIndex)
            {
                Debug.LogError("index�̒l���I�[�o�[���Ă��܂� index:" + seObjectIndex);
                return this;
            }
            if (offset == null) offset = Vector3.zero;
            
            //AudioSource�̐����Ɛݒ�
            AudioSource audioSourceSe = m_Pools[seObjectIndex].Get();
            audioSourceSe.transform.position = (Vector3)offset + parent.position;
            audioSourceSe.transform.parent = parent;
            audioSourceSe.outputAudioMixerGroup = m_MixerGroupSe;
            audioSourceSe.loop = loop;
            audioSourceSe.volume = volume;

            if (pitch != 9999)
            {
                //�s�b�`��ύX����
                if (m_MaxPitch < pitch) pitch = m_MaxPitch;
                else if (m_MinPitch > pitch) pitch = m_MinPitch;
                audioSourceSe.pitch = pitch;
            }

            bool is_Played = false;

            for (int i = 0; i < m_AudioDataBase.m_SEData.Length; i++)
            {
                if (m_AudioDataBase.m_SEData[i].Name == name && !is_Played)
                {
                    //Loop�Ȃ�Play
                    if (loop)
                    {
                        audioSourceSe.clip = m_AudioDataBase.m_SEData[i].AudioClip;
                        audioSourceSe.Play();
                    }
                    else audioSourceSe.PlayOneShot(m_AudioDataBase.m_SEData[i].AudioClip);
                    m_SeCount++;
                    is_Played = true;
                }
            }
            if (!audioSourceSe.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            //�������񂾂�A���g�ɂ��Ă��邱��AudioSource���폜����
            if (!loop)
            {
                m_CoroutineNumber++;
                Coroutine coroutine = StartCoroutine(Checking(audioSourceSe, m_CoroutineNumber, () =>
                {
                    m_Pools[seObjectIndex].Release(audioSourceSe);
                    m_SeCount--;
                }));
                m_CheckCoroutineDic.Add(m_CoroutineNumber, coroutine);
            }

            return this;
        }

        /// <summary>
        /// �v�f�ԍ���ݒ肵�A�w�肵���I�u�W�F�N�g����SE��炷
        /// ��������̂̓I�u�W�F�N�g��
        /// </summary>
        /// <param name="index">SE�̗v�f�ԍ�</param>
        /// <param name="parentObj">�e�ƂȂ�I�u�W�F�N�g</param>
        /// <param name="offset">�I�u�W�F�N�g�𐶐�����ʒu�̒���</param>
        /// <param name="seObjectIndex">�ǂ̃v���n�u���g�p���邩</param>
        /// <param name="pitch">���̍�������</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        /// <returns></returns>
        public AudioManager PlaySEAsChild(int index,Transform parent,Vector3? offset = null,int seObjectIndex = 0, float volume = 1f, float pitch = 9999, bool loop = false)
        {
            //����ȏ�AudioSource���������特��炳�Ȃ��悤�ɂ���
            if (m_MaxSeCount < m_SeCount) return this;

            //index�̒l��Length�̒l�𒴂��Ă�����G���[��\������
            if (m_SePrefabs.Length < seObjectIndex)
            {
                Debug.LogError("index�̒l���I�[�o�[���Ă��܂� index:" + seObjectIndex);
                return this;
            }
            
            //AudioSource�̐����Ɛݒ�
            AudioSource audioSourceSe = m_Pools[seObjectIndex].Get();
            audioSourceSe.transform.position = (Vector3)offset + parent.position;
            audioSourceSe.transform.parent = parent;
            audioSourceSe.outputAudioMixerGroup = m_MixerGroupSe;
            audioSourceSe.loop = loop;
            audioSourceSe.volume = volume;

            if (pitch != 9999)
            {
                //�s�b�`��ύX����
                if (m_MaxPitch < pitch) pitch = m_MaxPitch;
                else if (m_MinPitch > pitch) pitch = m_MinPitch;
                audioSourceSe.pitch = pitch;
            }

            //�z�񂪒����Ă�����G���[��\������
            if (m_AudioDataBase.m_SEData.Length <= index)
            {
                Debug.LogError($"�z�񂪒����Ă��܂��@�v�f��:{m_AudioDataBase.m_SEData.Length} index:{index}");
                return this;
            }

            //���[�v����ꍇ��Play�A���[�v���Ȃ��ꍇ��OneShot�ɂ���
            if (loop)
            {
                audioSourceSe.clip = m_AudioDataBase.m_SEData[index].AudioClip;
                audioSourceSe.Play();
            }
            else audioSourceSe.PlayOneShot(m_AudioDataBase.m_SEData[index].AudioClip);

            if (!audioSourceSe.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            //�������񂾂�A���g�ɂ��Ă��邱��AudioSource���폜����
            //���񂾍ۂɃR�[���o�b�N���ݒ肳��Ă���ꍇ�͂��̃R�[���o�b�N����������
            if (!loop)
            {
                m_CoroutineNumber++;
                Coroutine coroutine = StartCoroutine(Checking(audioSourceSe, m_CoroutineNumber, () =>
                {
                    m_Pools[seObjectIndex].Release(audioSourceSe);
                    m_SeCount--;
                }));
                m_CheckCoroutineDic.Add(m_CoroutineNumber, coroutine);
            }

            return this;
        }

        /// <summary>
        /// �w�肵���I�u�W�F�N�g����SE��炷
        /// ��������̂̓I�u�W�F�N�g�O
        /// </summary>
        /// <param name="name">SE�̖��O</param>
        /// <param name="parentObj">�w�肵���I�u�W�F�N�g</param>
        /// <param name="offset">�I�u�W�F�N�g�𐶐�����ʒu�̒���</param>
        /// <param name="seObjectIndex">�ǂ̃v���n�u���g�p���邩</param>
        /// <param name="pitch">���̍����̒���</param>
        /// <param name="loop">���[�v���邩�ǂ���</param>
        public AudioManager PlaySEFromObjectPosition(string name, GameObject parentObj, Vector3? offset = null, int seObjectIndex = 0, float volume = 1f, float pitch = 9999, bool loop = false)
        {
            //���������A�܂���None�Ɠ��͂����ꍇ�͏������Ȃ��悤�ɂ���
            if (name == "" || name == "None") return this;

            //����ȏ�AudioSource���������特��炳�Ȃ��悤�ɂ���
            if (m_MaxSeCount < m_SeCount) return this;
            //index�̒l��Length�̒l�𒴂��Ă�����G���[��\������
            if (m_SePrefabs.Length < seObjectIndex)
            {
                Debug.LogError("index�̒l���I�[�o�[���Ă��܂� index:" + seObjectIndex);
                return this;
            }
            if (offset == null) offset = Vector3.zero;

            //AudioSource�̐����Ɛݒ�
            AudioSource audioSourceSe = m_Pools[seObjectIndex].Get();
            audioSourceSe.transform.position = (Vector3)offset + parentObj.transform.position;
            audioSourceSe.outputAudioMixerGroup = m_MixerGroupSe;
            audioSourceSe.loop = loop;
            audioSourceSe.volume = volume;

            if (pitch != 9999)
            {
                //�s�b�`��ύX����
                if (m_MaxPitch < pitch) pitch = m_MaxPitch;
                else if (m_MinPitch > pitch) pitch = m_MinPitch;
                audioSourceSe.pitch = pitch;
            }

            bool is_Played = false;

            for (int i = 0; i < m_AudioDataBase.m_SEData.Length; i++)
            {
                if (m_AudioDataBase.m_SEData[i].Name == name && !is_Played)
                {
                    //Loop�Ȃ�Play
                    if (loop)
                    {
                        audioSourceSe.clip = m_AudioDataBase.m_SEData[i].AudioClip;
                        audioSourceSe.Play();
                    }
                    else audioSourceSe.PlayOneShot(m_AudioDataBase.m_SEData[i].AudioClip);
                    m_SeCount++;
                    is_Played = true;
                }
            }
            if (!audioSourceSe.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            //�������񂾂�A���g�ɂ��Ă��邱��AudioSource���폜����
            if (!loop)
            {
                m_CoroutineNumber++;
                Coroutine coroutine = StartCoroutine(Checking(audioSourceSe, m_CoroutineNumber, () =>
                {
                    m_Pools[seObjectIndex].Release(audioSourceSe);
                    m_SeCount--;
                }));
                m_CheckCoroutineDic.Add(m_CoroutineNumber, coroutine);
            }


            return this;
        }

        /// <summary>
        /// �v�f�ԍ���ݒ肵�A�w�肵���I�u�W�F�N�g����SE��炷
        /// ��������̂̓I�u�W�F�N�g�O
        /// </summary>
        /// <param name="index"></param>
        /// <param name="parentObj"></param>
        /// <param name="offset"></param>
        /// <param name="seObjectIndex"></param>
        /// <param name="pitch"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public AudioManager PlaySEFromObjectPosition(int index,GameObject parentObj,Vector3? offset = null,int seObjectIndex = 0,float volume = 1f,float pitch = 9999,bool loop = false)
        {
            //����ȏ�AudioSource���������特��炳�Ȃ��悤�ɂ���
            if (m_MaxSeCount < m_SeCount) return this;

            //index�̒l��Length�̒l�𒴂��Ă�����G���[��\������
            if (m_SePrefabs.Length < seObjectIndex)
            {
                Debug.LogError("index�̒l���I�[�o�[���Ă��܂� index:" + seObjectIndex);
                return this;
            }

            //AudioSource�̐����Ɛݒ�
            AudioSource audioSourceSe = m_Pools[seObjectIndex].Get();
            audioSourceSe.transform.position = (Vector3)offset + parentObj.transform.position;
            audioSourceSe.outputAudioMixerGroup = m_MixerGroupSe;
            audioSourceSe.volume = volume;
            audioSourceSe.loop = loop;

            if (pitch != 9999)
            {
                //�s�b�`��ύX����
                if (m_MaxPitch < pitch) pitch = m_MaxPitch;
                else if (m_MinPitch > pitch) pitch = m_MinPitch;
                audioSourceSe.pitch = pitch;
            }

            //�z�񂪒����Ă�����G���[��\������
            if (m_AudioDataBase.m_SEData.Length <= index)
            {
                Debug.LogError($"�z�񂪒����Ă��܂��@�v�f��:{m_AudioDataBase.m_SEData.Length} index:{index}");
                return this;
            }

            //���[�v����ꍇ��Play�A���[�v���Ȃ��ꍇ��OneShot�ɂ���
            if (loop)
            {
                audioSourceSe.clip = m_AudioDataBase.m_SEData[index].AudioClip;
                audioSourceSe.Play();
            }
            else audioSourceSe.PlayOneShot(m_AudioDataBase.m_SEData[index].AudioClip);

            if (!audioSourceSe.isPlaying)
                Debug.LogError("Clip��������܂���ł��� Name:" + name);

            //�������񂾂�A���g�ɂ��Ă��邱��AudioSource���폜����
            //���񂾍ۂɃR�[���o�b�N���ݒ肳��Ă���ꍇ�͂��̃R�[���o�b�N����������
            if (!loop)
            {
                m_CoroutineNumber++;
                Coroutine coroutine = StartCoroutine(Checking(audioSourceSe, m_CoroutineNumber, () =>
                {
                    m_Pools[seObjectIndex].Release(audioSourceSe);
                    m_SeCount--;
                }));
                m_CheckCoroutineDic.Add(m_CoroutineNumber, coroutine);
            }

            return this;
        }

        /// <summary>
        /// �������񂾎��ɃR�[���o�b�N����
        /// </summary>
        /// <param name="audio">���g��AudioSource�R���|�[�l���g</param>
        /// <param name="callback">�I�����ɌĂяo��</param>
        private IEnumerator Checking(AudioSource audio, int coroutineNum, UnityAction callback)
        {
            yield return new WaitWhile(() => audio.isPlaying);

            callback();
            m_CheckCoroutineDic.Remove(coroutineNum);
        }

        /// <summary>
        /// �V�[�����ǂݍ��܂ꂽ�Ƃ��ɌĂ΂��
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Debugger.Log("�V�[���ǂݍ��݊���"+ scene.name);
            m_CheckCoroutineDic = new();
            m_CoroutineNumber = 0;

            m_AudioObjectList.Clear();
            m_SeCount = 0;
        }

        /// <summary>
        /// �V�[�����ǂݍ��܂��O�ɌĂ΂��
        /// </summary>
        /// <param name="scene"></param>
        private void OnSceneUnLoaded(Scene scene)
        {
            //�R���[�`�����쓮���ĂȂ��ꍇError�ɂȂ�
            foreach (Coroutine coroutine in m_CheckCoroutineDic.Values)
            {
                StopCoroutine(coroutine);
            }

            Debugger.Log("�V�[���ǂݍ��݂܂�" + scene.name);
            foreach (GameObject obj in m_AudioObjectList)
            {
                Destroy(obj);
            }
        }

        /// <summary>
        /// �����ꎞ��~����
        /// </summary>
        public void PauseBgm()
             => m_AudioSourceBgm.Pause();

        /// <summary>
        /// �����~����
        /// </summary>
        public void StopBgm()
            => m_AudioSourceBgm.Stop();

        /// <summary>
        /// �����ĊJ����
        /// </summary>
        public void ReStartBgm()
          => m_AudioSourceBgm.Play();

        /// <summary>
        /// ���ׂẲ��ʂ𒲐߂���
        /// </summary>
        /// <param name="volume">����</param>
        public void SetAllVolume(float volume)
            => m_Mixer.SetFloat("Master", volume);

        /// <summary>
        /// ���ʒ���
        /// </summary>
        /// <param name="volume">����</param>
        public AudioManager SetVolumeBgm(float volume)
        {
            m_AudioSourceBgm.volume = volume;
            return this;
        }

        /// <summary>
        /// BGM�S�̂̉��ʂ�ύX����
        /// </summary>
        /// <param name="volume">����</param>
        public void SetVolumeBgmMixer(float volume)
            => m_Mixer.SetFloat("BGM", volume);

        /// <summary>
        /// SE�S�̂̉��ʂ�ύX����
        /// </summary>
        /// <param name="volume">����</param>
        public void SetVolumeSeMixer(float volume)
            => m_Mixer.SetFloat("SE", volume);

        /// <summary>
        /// ���X�ɉ������������Ă���
        /// </summary>
        /// <param name="time">�������Ȃ鎞��</param>
        public AudioManager FadeInBgm(float time, float volume = 1)
        {
            m_FadeSpeed = m_AudioSourceBgm.volume / time;
            StartCoroutine(FadeInBgm(volume));
            return this;
        }

        /// <summary>
        /// ���X�ɉ���傫�����Ă���
        /// </summary>
        /// <param name="time">�傫���Ȃ鎞��</param>
        public AudioManager FadeOutBgm(float time, float volume = 0)
        {
            m_FadeSpeed = m_AudioSourceBgm.volume / time;
            StartCoroutine(FadeOutBgm(volume));
            return this;
        }

        private IEnumerator FadeInBgm(float volume)
        {
            float defaultVolume = 0;
            //default�̉��ʂ��L�����Ă���
            if (volume == 1) defaultVolume = m_AudioSourceBgm.volume;
            else defaultVolume = volume;

            while (m_AudioSourceBgm.volume > 0)
            {
                yield return null;
                m_AudioSourceBgm.volume -= m_FadeSpeed * Time.deltaTime;
            }
            StopBgm();
            m_AudioSourceBgm.volume = defaultVolume;
        }

        private IEnumerator FadeOutBgm(float volume)
        {
            float defaultVolume = 0;
            //default�̉��ʂ��L�����Ă���
            if (volume == 0) defaultVolume = m_AudioSourceBgm.volume;
            else defaultVolume = volume;

            m_AudioSourceBgm.volume = 0;
            while (m_AudioSourceBgm.volume < defaultVolume)
            {
                yield return null;
                m_AudioSourceBgm.volume += m_FadeSpeed * Time.deltaTime;
            }
            m_AudioSourceBgm.volume = defaultVolume;
        }
    }
}