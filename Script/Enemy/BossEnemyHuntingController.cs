using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Cinemachine;
using KD.Tweening;

public class BossEnemyHuntingController : MonoBehaviour
{
    [SerializeField]
    private VirtualCamera m_BossCamera;

    [SerializeField]
    private float m_ClearTimeScale;

    [System.Serializable]
    private struct TweenCameraData
    {
        public float StartValue;
        public float EndValue;
        public float Duration;
    }

    [SerializeField]
    private TweenCameraData[] m_TweenCameraDatas = new TweenCameraData[3];

    [SerializeField]
    private float m_TweenCameraDistanceValue = 7; //Tweenする際のカメラのDistanceの距離の離れる量
    [SerializeField]
    private float m_TweenCameraDistanceDuration = 5f;

    private static bool m_IsMovie;
    public static bool IsMovie => m_IsMovie;

    private void Awake()
    {
        m_IsMovie = false;
    }

    public void BossHunted()
    {
        Time.timeScale = m_ClearTimeScale;

        m_BossCamera.Priority = 10;
        m_IsMovie = true;

        //最初の値に変更する
        m_BossCamera.Aim.m_HorizontalAxis.Value = m_TweenCameraDatas[0].StartValue;

        var sequence = new KDSequence();

        //TweenをSequenceに追加
        sequence.Append(m_BossCamera.TweenHorizontalAxis(m_TweenCameraDatas[0].EndValue, m_TweenCameraDatas[0].Duration)
                        .SetUnscaled().OnComplete(()=> m_BossCamera.Aim.m_HorizontalAxis.Value = m_TweenCameraDatas[1].StartValue));
        sequence.Append(m_BossCamera.TweenHorizontalAxis(m_TweenCameraDatas[1].EndValue, m_TweenCameraDatas[1].Duration)
                        .SetUnscaled().OnComplete(() => m_BossCamera.Aim.m_HorizontalAxis.Value = m_TweenCameraDatas[2].StartValue));
        sequence.Append(m_BossCamera.TweenDistance(m_TweenCameraDistanceValue, m_TweenCameraDistanceDuration)
                            .SetRelative().SetUnscaled().SetEase(Ease.OutCubic)
                            .OnComplete(() =>
                            {
                                     m_BossCamera.Priority = 0;
                                     Time.timeScale = 1;
                                     m_IsMovie = false;
                            }));
        //実行する
        sequence.Play();
    }
}
