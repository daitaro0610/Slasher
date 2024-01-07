using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening;

public class TitleCameraController : MonoBehaviour
{
    [System.Serializable]
    public struct RootData
    {
        public Transform Transform;
        public float Duration;
        public float NextWaitTime;
        public Ease m_CurrentEasing;
    }

    [SerializeField]
    private RootData[] m_Root;

    private void Awake()
    {
        StartCoroutine(CameraMoveCoroutine());
    }
    private IEnumerator CameraMoveCoroutine()
    {
        foreach (var root in m_Root)
        {
            transform.TweenMove(root.Transform.position, root.Duration).SetLink(gameObject).SetEase(root.m_CurrentEasing);
            transform.TweenRotate(root.Transform.eulerAngles, root.Duration).SetLink(gameObject).SetEase(root.m_CurrentEasing);
            yield return new WaitForSeconds(root.Duration + root.NextWaitTime);
        }
    }
}
