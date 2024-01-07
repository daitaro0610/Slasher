using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopSystem : MonoBehaviour
{
    [SerializeField]
    private float m_TimeScale;

    public HitStopSystem SetTimeScale(float scale)
    {
        m_TimeScale = scale;
        return this;
    }

    public void HitStop(float time)
    {
        StartCoroutine(HitStopCoroutine(time));
    }

    private IEnumerator HitStopCoroutine(float time)
    {
        Time.timeScale = m_TimeScale;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }
}
