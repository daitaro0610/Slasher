using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset m_InputActionAsset;

    private RebindSaveSystem m_SaveSystem;

    private void Awake()
    {
        m_SaveSystem = new();
        m_SaveSystem.Load(m_InputActionAsset);
    }

    private void OnDestroy()
    {
        m_SaveSystem.Save(m_InputActionAsset);
    }
}
