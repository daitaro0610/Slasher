using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSettings : MonoBehaviour
{
    [SerializeField]
    CursorLockMode m_LockMode;

    [SerializeField]
    private bool m_IsVisible;

    public Texture2D m_CursorTex;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = m_LockMode;
        Cursor.visible = m_IsVisible;
        Cursor.SetCursor(m_CursorTex, Vector2.zero, CursorMode.Auto);
    }

    public void ChangeLockMode(CursorLockMode mode)
    {
        Cursor.lockState = mode;
        m_LockMode = mode;
    }

    public void IsVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        m_IsVisible = isVisible;
    }
}
