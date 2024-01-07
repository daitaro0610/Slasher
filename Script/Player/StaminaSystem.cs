using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    private Slider m_Slider;

    [SerializeField]
    private float m_StaminaMaxValue;
    private float m_StaminaValue;
    public float StaminaMaxValue => m_StaminaMaxValue;
    public float StaminaValue => m_StaminaValue;

    private bool m_IsUse;

    [SerializeField,Header("スタミナが消費されていく速度　x/秒")]
    private float m_StaminaDecreaseTime = 1f;
    [SerializeField,Header("スタミナが回復する速度　x/秒")]
    private float m_StaminaIncreaseTime = 1f;
    private void Awake()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.maxValue = m_StaminaMaxValue;
        m_StaminaValue = m_StaminaMaxValue;
        m_IsUse = false;
    }

    private void Start()
    {
    }

    private void Update()
    {
        UpdateSliderValue(Time.deltaTime);
    }

    public bool SetUse(bool isbool)
    {
        if (m_StaminaValue <= 0) return false;
        m_IsUse = isbool;
        return true;
    }

    public bool UseStamina(float value)
    {
        if(0 > m_StaminaValue - value)
        {
            //スタミナが足りなかったらFalseを返却する
            return false;
        }

        m_StaminaValue -= value;
        m_Slider.value = m_StaminaValue;

        return true;
    }

    /// <summary>
    /// スタミナの上昇減少を更新する処理　IsUseがTrueなら消費していく
    /// </summary>
    /// <param name="time"></param>
    private void UpdateSliderValue(float time)
    {
        if (m_IsUse)
            m_StaminaValue -= time * m_StaminaDecreaseTime;
        else m_StaminaValue += time * m_StaminaIncreaseTime;

        if (m_StaminaMaxValue < m_StaminaValue) m_StaminaValue = m_StaminaMaxValue;
        else if (0 >= m_StaminaValue)
        {
            m_IsUse = false;
            m_StaminaValue = 0;
        }
        m_Slider.value = m_StaminaValue;
    }
}
