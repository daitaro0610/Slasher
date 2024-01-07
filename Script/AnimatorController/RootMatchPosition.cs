using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinMaxSlider;
using KD.Tweening;
using KD.Tweening.Plugin.Options;

public class RootMatchPosition : StateMachineBehaviour
{
    [System.Serializable]
    public struct MoveData
    {
        [MinMaxSlider(0, 1)]
        public Vector2 MoveDurationRange;
        public float MoveVelocityForward;
        [HideInInspector]
        public bool IsMoved;

        public Ease m_Easing;
    }

    [SerializeField]
    private MoveData[] m_MoveData;

    private float m_Length;

    private KDTween<Vector3,VectorOptions> m_Tween;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < m_MoveData.Length; i++)
        {
            m_MoveData[i].IsMoved = false;
        }
        m_Length = stateInfo.length;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < m_MoveData.Length; i++)
        {
            if (m_MoveData[i].IsMoved) continue;

            //正規化した値のうち、m_MoveDurationRangeの範囲内であれば動かす
            if (m_MoveData[i].MoveDurationRange.x < stateInfo.normalizedTime)
            {
                m_MoveData[i].IsMoved = true;
                //動かす時間を計算する
                float duration = (m_MoveData[i].MoveDurationRange.y - m_MoveData[i].MoveDurationRange.x) * m_Length;
                Vector3 velocityForward = animator.transform.forward * m_MoveData[i].MoveVelocityForward;

                //Tweenを使用して自身を動かす
                m_Tween = (KDTween<Vector3,VectorOptions>)animator.transform.TweenMove(velocityForward, duration)
                    .SetRelative()
                    .SetEase(m_MoveData[i].m_Easing);

                return;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        if (m_Tween != null)
        {
            m_Tween.Kill();
        }
    }
}
