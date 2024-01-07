using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.ObjectPool;

namespace KD
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager instance;

        [SerializeField]
        private EffectDataBase m_EffectDataBase;
        private EffectDataBase.EffectData[] m_EffectDatas;

        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                m_EffectDatas = m_EffectDataBase.m_EffectData;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// �G�t�F�N�g���o��
        /// </summary>
        /// <param name="name"></param>
        public ParticleSystem Play(string name, GameObject instantiateObj, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null)
        {
            bool isNull = true;
            Vector3 offSet = Vector3.zero;
            if (name == "None" || name == "") return null;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;
            for (int i = 0; i < m_EffectDatas.Length; i++)
            {
                //�G�t�F�N�g�ƈ����������������ꍇ�I�u�W�F�N�g�𐶐�����
                if (m_EffectDatas[i].Name == name)
                {
                    isNull = false;
                    //Rotation�ɒl���Ȃ�������I�u�W�F�N�g�̊p�x�����̂܂ܔ��f�����邪�A�l����ꂽ�ꍇ�͊p�x�𔽉f������
                    if (rotation == null)
                        newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                           instantiateObj.transform.position, Quaternion.identity, instantiateObj.transform);
                    else newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                       instantiateObj.transform.position, Quaternion.Euler((Vector3)rotation + instantiateObj.transform.eulerAngles)
                       , instantiateObj.transform);

                    newParticle.transform.localPosition += offSet;

                    newParticle.transform.localScale = new Vector3(size, size, size);
                    //�p�[�e�B�N���𓮂���
                    newParticle.Play();
                    break;
                }
            }

            //�Đ�����Ȃ��ꍇ�̓G���[��\������
            if (isNull) Debug.LogError("Particle��������܂���ł��� Name:" + name);

            //time��0�̂Ƃ��͍폜���鏈�������Ȃ��B����ȊO�Ȃ�A��莞�Ԃ�����������鏈��������
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }

        /// <summary>
        /// �G�t�F�N�g���o��
        /// </summary>
        /// <param name="index">�v�f�ԍ�</param>
        public ParticleSystem Play(int index, GameObject instantiateObj, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null)
        {
            Vector3 offSet = Vector3.zero;
            if (index >= m_EffectDatas.Length) return null;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;

            //Rotation�ɒl���Ȃ�������I�u�W�F�N�g�̊p�x�����̂܂ܔ��f�����邪�A�l����ꂽ�ꍇ�͊p�x�𔽉f������
            if (rotation == null)
                newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
                   instantiateObj.transform.position, Quaternion.identity, instantiateObj.transform);
            else newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
               instantiateObj.transform.position, Quaternion.Euler((Vector3)rotation + instantiateObj.transform.eulerAngles)
               , instantiateObj.transform);

            newParticle.transform.localPosition += offSet;

            newParticle.transform.localScale = new Vector3(size, size, size);
            //�p�[�e�B�N���𓮂���
            newParticle.Play();

            //time��0�̂Ƃ��͍폜���鏈�������Ȃ��B����ȊO�Ȃ�A��莞�Ԃ�����������鏈��������
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }

        /// <summary>
        /// �G�t�F�N�g��C�ӂ̈ʒu�ɕ\������
        /// </summary>
        /// <param name="name">���O</param>
        /// <param name="Position">�ʒu</param>
        /// <param name="time">��������</param>
        public ParticleSystem PlayPosition(string name, Vector3 Position, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null,bool isLocal = false)
        {
            Vector3 offSet = Vector3.zero;

            if (name == "None" || name == "") return null;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;
            for (int i = 0; i < m_EffectDatas.Length; i++)
            {
                //�G�t�F�N�g�ƈ����������������ꍇ�I�u�W�F�N�g�𐶐�����
                if (m_EffectDatas[i].Name == name)
                {
                    //Rotation�ɒl���Ȃ�������I�u�W�F�N�g�̊p�x�����̂܂ܔ��f�����邪�A�l����ꂽ�ꍇ�͊p�x�𔽉f������
                    if (rotation == null)
                        newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                           Position + offSet, Quaternion.identity);
                    else newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                       Position + offSet, Quaternion.Euler((Vector3)rotation), gameObject.transform);

                    newParticle.transform.localScale = new Vector3(size, size, size);
                    //�p�[�e�B�N���𓮂���
                    newParticle.Play();

                    break;
                }
            }

            //�Đ�����Ȃ��ꍇ�̓G���[��\������
            if (newParticle == null) Debug.LogError("Particle��������܂���ł��� Name:" + name);

            //time��0�̂Ƃ��͍폜���鏈�������Ȃ��B����ȊO�Ȃ�A��莞�Ԃ�����������鏈��������
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }

        /// <summary>
        /// �G�t�F�N�g��C�ӂ̈ʒu�ɕ\������
        /// </summary>
        /// <param name="index">�v�f�ӕ�</param>
        /// <param name="Position">�ʒu</param>
        /// <param name="time">��������</param>
        public ParticleSystem PlayPosition(int index, Vector3 Position, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null, bool isLocal = false)
        {
            Vector3 offSet = Vector3.zero;

            if (index >= m_EffectDatas.Length) return null ;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;

            //Rotation�ɒl���Ȃ�������I�u�W�F�N�g�̊p�x�����̂܂ܔ��f�����邪�A�l����ꂽ�ꍇ�͊p�x�𔽉f������
            if (rotation == null)
                newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
                   Position + offSet, Quaternion.identity);
            else newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
               Position + offSet, Quaternion.Euler((Vector3)rotation), gameObject.transform);

            newParticle.transform.localScale = new Vector3(size, size, size);
            //�p�[�e�B�N���𓮂���
            newParticle.Play();

            //time��0�̂Ƃ��͍폜���鏈�������Ȃ��B����ȊO�Ȃ�A��莞�Ԃ�����������鏈��������
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }


        /// <summary>
        /// ��莞�Ԍ�ɏ����鏈��
        /// </summary>
        /// <param name="particleSystem"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator DeleteCoroutine(ParticleSystem particleSystem, float time)
        {
            yield return new WaitForSeconds(time);
            particleSystem.Stop();
        }


    }
}