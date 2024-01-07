using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Tweening;
public class PlayerHpBar : MonoBehaviour
{
    private PlayerHealth m_Health;
    private int m_CurrentValue;
    private int m_MaxValue;

    private Slider m_HpBar;
    private void Awake()
    {
        m_HpBar = GetComponent<Slider>();
        GameObject playerObj = GameObject.FindGameObjectWithTag(TagName.Player);
        m_Health = playerObj.GetComponent<PlayerHealth>();
        m_MaxValue = m_Health.GetHealth();
    }

    private void Update()
    {
        if (m_CurrentValue != m_Health.GetHealth())
        {
            Damage();
            m_CurrentValue = m_Health.GetHealth();
        }
    }


    private void Damage()
    {
        m_HpBar.TweenValue(m_Health.GetHealth() / (float)m_MaxValue, 0.2f);
    }
}
