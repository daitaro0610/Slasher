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
        /// エフェクトを出す
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
                //エフェクトと引数が同名だった場合オブジェクトを生成する
                if (m_EffectDatas[i].Name == name)
                {
                    isNull = false;
                    //Rotationに値がなかったらオブジェクトの角度をそのまま反映させるが、値を入れた場合は角度を反映させる
                    if (rotation == null)
                        newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                           instantiateObj.transform.position, Quaternion.identity, instantiateObj.transform);
                    else newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                       instantiateObj.transform.position, Quaternion.Euler((Vector3)rotation + instantiateObj.transform.eulerAngles)
                       , instantiateObj.transform);

                    newParticle.transform.localPosition += offSet;

                    newParticle.transform.localScale = new Vector3(size, size, size);
                    //パーティクルを動かす
                    newParticle.Play();
                    break;
                }
            }

            //再生されない場合はエラーを表示する
            if (isNull) Debug.LogError("Particleが見つかりませんでした Name:" + name);

            //timeが0のときは削除する処理をしない。それ以外なら、一定時間たったら消える処理をする
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }

        /// <summary>
        /// エフェクトを出す
        /// </summary>
        /// <param name="index">要素番号</param>
        public ParticleSystem Play(int index, GameObject instantiateObj, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null)
        {
            Vector3 offSet = Vector3.zero;
            if (index >= m_EffectDatas.Length) return null;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;

            //Rotationに値がなかったらオブジェクトの角度をそのまま反映させるが、値を入れた場合は角度を反映させる
            if (rotation == null)
                newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
                   instantiateObj.transform.position, Quaternion.identity, instantiateObj.transform);
            else newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
               instantiateObj.transform.position, Quaternion.Euler((Vector3)rotation + instantiateObj.transform.eulerAngles)
               , instantiateObj.transform);

            newParticle.transform.localPosition += offSet;

            newParticle.transform.localScale = new Vector3(size, size, size);
            //パーティクルを動かす
            newParticle.Play();

            //timeが0のときは削除する処理をしない。それ以外なら、一定時間たったら消える処理をする
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }

        /// <summary>
        /// エフェクトを任意の位置に表示する
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="Position">位置</param>
        /// <param name="time">消す時間</param>
        public ParticleSystem PlayPosition(string name, Vector3 Position, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null,bool isLocal = false)
        {
            Vector3 offSet = Vector3.zero;

            if (name == "None" || name == "") return null;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;
            for (int i = 0; i < m_EffectDatas.Length; i++)
            {
                //エフェクトと引数が同名だった場合オブジェクトを生成する
                if (m_EffectDatas[i].Name == name)
                {
                    //Rotationに値がなかったらオブジェクトの角度をそのまま反映させるが、値を入れた場合は角度を反映させる
                    if (rotation == null)
                        newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                           Position + offSet, Quaternion.identity);
                    else newParticle = Instantiate(m_EffectDatas[i].ParticleSystem,
                       Position + offSet, Quaternion.Euler((Vector3)rotation), gameObject.transform);

                    newParticle.transform.localScale = new Vector3(size, size, size);
                    //パーティクルを動かす
                    newParticle.Play();

                    break;
                }
            }

            //再生されない場合はエラーを表示する
            if (newParticle == null) Debug.LogError("Particleが見つかりませんでした Name:" + name);

            //timeが0のときは削除する処理をしない。それ以外なら、一定時間たったら消える処理をする
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }

        /// <summary>
        /// エフェクトを任意の位置に表示する
        /// </summary>
        /// <param name="index">要素晩報</param>
        /// <param name="Position">位置</param>
        /// <param name="time">消す時間</param>
        public ParticleSystem PlayPosition(int index, Vector3 Position, float time = 0.0f, float size = 1, Vector3? rotation = null, Vector3? OffSet = null, bool isLocal = false)
        {
            Vector3 offSet = Vector3.zero;

            if (index >= m_EffectDatas.Length) return null ;

            if (OffSet != null) offSet = (Vector3)OffSet;

            ParticleSystem newParticle = null;

            //Rotationに値がなかったらオブジェクトの角度をそのまま反映させるが、値を入れた場合は角度を反映させる
            if (rotation == null)
                newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
                   Position + offSet, Quaternion.identity);
            else newParticle = Instantiate(m_EffectDatas[index].ParticleSystem,
               Position + offSet, Quaternion.Euler((Vector3)rotation), gameObject.transform);

            newParticle.transform.localScale = new Vector3(size, size, size);
            //パーティクルを動かす
            newParticle.Play();

            //timeが0のときは削除する処理をしない。それ以外なら、一定時間たったら消える処理をする
            if (time != 0.0f)
                StartCoroutine(DeleteCoroutine(newParticle, time));

            return newParticle;
        }


        /// <summary>
        /// 一定時間後に消える処理
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