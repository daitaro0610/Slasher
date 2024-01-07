using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Cinemachine;
using KD.Tweening;

public class ClearEffectController : MonoBehaviour
{
    [SerializeField]
    private VirtualCamera m_ClearCamera;
    [SerializeField]
    private float m_CameraMoveValue;
    [SerializeField]
    private float m_CameraMoveDuration;

    [SerializeField]
    private PlayerController m_Player;

    [SerializeField]
    private GameObject m_UiCanvasObjcet;

    [SerializeField]
    private ClearEffectDisplay m_ClearEffectDisplay; //�N���A�p�̃G�t�F�N�gUi�̏���
    [SerializeField]
    private float m_ClearEffectDisplayWaitTime;

    public void OnClear()
    {
        m_UiCanvasObjcet.SetActive(false);

        m_ClearCamera.Priority = 100;

        KDTweener.To(
            () => m_ClearCamera.Aim.m_HorizontalAxis.Value,
            (x) => m_ClearCamera.Aim.m_HorizontalAxis.Value = x,
            m_CameraMoveValue,
            m_CameraMoveDuration
            )
            .SetRelative()
            .SetEase(Ease.OutQuart);

        //Ui�p�̃N���A�G�t�F�N�g�����s����
        _ = Wait.WaitTime(m_ClearEffectDisplayWaitTime,
            () => StartCoroutine(m_ClearEffectDisplay.OnClearEffectDisplay()));

        m_Player.Clear();
    }

}
