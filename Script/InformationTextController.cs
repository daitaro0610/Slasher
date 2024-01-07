using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationTextController : MonoBehaviour
{
    [System.Serializable]
    public struct InformationTextData
    {
        public GameObject InformationTextObj;
        public bool IsActive;
    }

    [SerializeField]
    private InformationTextData[] m_IdleData;

    [SerializeField]
    private InformationTextData[] m_AttackData;

    public static InformationTextController Instance => instance;
    private static InformationTextController instance;


    private void Awake()
    {
        IdleStart();
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AttackStart()
    {
        foreach(InformationTextData data in m_AttackData)
        {
            data.InformationTextObj.SetActive(data.IsActive);
        }
    }

    public void IdleStart()
    {
        foreach (InformationTextData data in m_IdleData)
        {
            data.InformationTextObj.SetActive(data.IsActive);
        }
    }
}
