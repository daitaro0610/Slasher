using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Tweening;


public class KDTweenTest : MonoBehaviour
{
    [SerializeField]
    private float m_Strength = 5f;

    [SerializeField]
    private float m_Vibrato = 10f;

    [SerializeField]
    private Vector2 m_RandomOffset;

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.TweenShakePositionX(m_Strength, m_Vibrato, 1);
        }

    }
}
