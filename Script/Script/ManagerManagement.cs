using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerManagement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_Managers;

    private void Awake()
    {
        foreach(GameObject manager in m_Managers)
        {
            Instantiate(manager);
        }
    }
}
