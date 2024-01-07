using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KD.ObjectPool;

public class DamageEffectManager : MonoBehaviour
{
    private static DamageEffectManager m_Instance;
    public static DamageEffectManager Instance => m_Instance;

    [SerializeField]
    private GameObject m_TextEffectObject;

    private ObjectPool<DamageText> m_Pool;

    [System.Serializable]
    private struct DamageData
    {
        public int DamageValue;
        public Color DamageColor;
    }

    [SerializeField]
    private DamageData m_MinDamage;
    [SerializeField]
    private DamageData m_MaxDamage;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else Destroy(gameObject);

        //オブジェクトプールの作成
        m_Pool = new(m_TextEffectObject, transform);
    }

    public void InstantiateDamageText(int damageValue, Vector3 position)
    {
        var damageText = m_Pool.Get();
        Color textColor = ChangeColorDamageValue(damageValue);
        damageText.Init(damageValue.ToString(), position, textColor);
    }

    private Color ChangeColorDamageValue(int damageValue)
    {
        float t = (float)(damageValue - m_MinDamage.DamageValue) / m_MaxDamage.DamageValue;
        return Color.Lerp(m_MinDamage.DamageColor, m_MaxDamage.DamageColor, t);
    }

    public void Release(DamageText damageText)
    {
        m_Pool.Release(damageText);
    }
}
