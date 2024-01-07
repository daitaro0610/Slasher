using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KD;
using KD.Tweening;
using KD.Cinemachine;
public class GameOverController : MonoBehaviour
{
    [SerializeField]
    private VirtualCamera m_PlayerDeathCamera;

    [SerializeField]
    private float m_CameraMoveTime;

    [SerializeField]
    private GameObject m_UiCanvasObject;

    private bool m_IsSubmitButtonPressed;

    private PlayerGame m_Input;

    private void Awake()
    {
        m_IsSubmitButtonPressed = false;
        m_PlayerDeathCamera.Priority = -10;
    }

    /// <summary>
    /// カメラを回転させた後、決定ボタンを押したらシーンを遷移する
    /// </summary>
    public void GameOver()
    {
        MusicController.Instance.PlayGameOverBgm();
        m_PlayerDeathCamera.Priority = 100;

        m_Input = new();
        m_Input.UI.Decision.started += OnSubmitCallback;
        m_Input.Enable();

        m_UiCanvasObject.SetActive(false);

        //数秒間かけてカメラを一周させる
        KDTweener.To(
            () => m_PlayerDeathCamera.Aim.m_HorizontalAxis.Value,
            (x) => m_PlayerDeathCamera.Aim.m_HorizontalAxis.Value = x,
            359f,
            m_CameraMoveTime
            )
            .SetLink(m_PlayerDeathCamera.gameObject);

        StartCoroutine(GameOverCoroutine()); 
    }

    private IEnumerator GameOverCoroutine()
    {
        while (true)
        {
            if (m_IsSubmitButtonPressed)
                break;
            yield return null;
        }

        MusicController.Instance.StopFadeBgm();
        //現在のシーンを再読み込み
        SceneManager.instance.PlayLoad(SceneManager.instance.CurrentPlaySceneNum);
    }

    private void OnSubmitCallback(InputAction.CallbackContext callbackContext)
    {
        m_IsSubmitButtonPressed = true;
    }
}
