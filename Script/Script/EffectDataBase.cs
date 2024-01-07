using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "CreateData/EffectData")]
public class EffectDataBase :ScriptableObject
{
    public EffectData[] m_EffectData;

    [System.Serializable]
    public struct EffectData
    {

        [SerializeField]
        private string m_Name;

        [SerializeField]
        ParticleSystem m_ParticleSystem;


        public string Name => m_Name;

        public ParticleSystem ParticleSystem => m_ParticleSystem;

    }
}
