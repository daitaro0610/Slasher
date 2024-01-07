using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionManager : MonoBehaviour
{
    private Queue<Action> m_FunctionQueue;
    private bool m_IsProcessing;

    [SerializeField, Header("処理待機時間"),Min(0)]
    private float m_FunctionInterval = 0.1f;

    private static FunctionManager m_instance;
    public static FunctionManager Instance => m_instance;

    private WaitForSeconds WaitForSeconds;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            m_FunctionQueue = new Queue<Action>();
            WaitForSeconds = new WaitForSeconds(m_FunctionInterval);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 処理をため込んでおく　終わったら次の処理に移る
    /// </summary>
    /// <param name="function"></param>
    public void AddFunction(Action function)
    {
        m_FunctionQueue.Enqueue(function);
        TryProcessNextFunction();
    }

    private void TryProcessNextFunction()
    {
        if (!m_IsProcessing && m_FunctionQueue.Count > 0)
        {
            m_IsProcessing = true;
            Action nextFunction = m_FunctionQueue.Dequeue();
            StartCoroutine(InvokeFunction(nextFunction));
        }
    }

    private IEnumerator InvokeFunction(Action function)
    {
        function.Invoke();

        // m_FunctionInterval秒待機
        yield return WaitForSeconds;

        m_IsProcessing = false;
        TryProcessNextFunction();
    }
}
