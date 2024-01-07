using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistentDataSlider : Slider
{
    private string m_KeyName;

    protected override void Awake()
    {
        m_KeyName = gameObject.name.ToString();
        value = PlayerPrefs.GetFloat(m_KeyName,0);
    }

    protected override void OnDestroy()
    {
        PlayerPrefs.SetFloat(m_KeyName, value);   
    }

}
