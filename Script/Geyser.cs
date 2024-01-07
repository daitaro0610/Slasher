using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    [SerializeField, Header("%"), Range(1, 100)]
    private int m_Probability = 5;

    [SerializeField]
    private  int m_EvaluationInterval = 10;
    private float m_Timer;

    [SerializeField]
    private ParticleSystem m_GeyserEffect;

    // Start is called before the first frame update
    void Start()
    {
        m_Timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer += Time.deltaTime;

        //指定秒に一回、一定の確率で間欠泉が起動する
        if (m_Timer > m_EvaluationInterval)
        {
            m_Timer = 0;
            CheckGeyser();
        }
    }

    private void CheckGeyser()
    {
        int rand = Random.Range(0, 100);

        if (rand <= m_Probability)
        {
            GeyserPlay();
        }
    }

    public void GeyserPlay()
    {
        m_Timer = 0;
        m_GeyserEffect.time = 0;
        m_GeyserEffect.Play();
    }
}
