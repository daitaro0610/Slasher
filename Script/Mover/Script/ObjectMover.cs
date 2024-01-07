using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening;

namespace KD.Tweening.Mover
{
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField, Header("最初にすべての処理を実行するかどうか")]
        private bool m_PlayOnAwake = false;
        [SerializeField, Header("PlayOnAwakeをOnにしたときの待ち時間")]
        private float m_FirstWaitTime = 0;

        [SerializeField]
        private Movement[] m_Movement;

        public enum State
        {
            Transform,
            RectTransform,
        }

        public enum MoveSettings
        {
            MoveAndRotate,
            Move,
            Rotate,
        }

        [System.Serializable]
        public class Movement
        {
            public GameObject MoveObject;

            [SerializeField, Tooltip("終点")]
            private Vector3 m_EndPosition;
            [SerializeField, Tooltip("角度")]
            private Vector3 m_Angles;

            [SerializeField, Tooltip("回転をさせるかさせないか")]
            private MoveSettings m_Settings = MoveSettings.MoveAndRotate;
            [SerializeField, Tooltip("移動する時間")]
            private float m_MoveTime;

            [SerializeField, Header("移動時の移動のEase")]
            private Ease m_MoveEase = Ease.Linear;
            [SerializeField, Header("回転時の移動のEase")]
            private Ease m_RotateEase = Ease.Linear;

            public Vector3 EndPosition => m_EndPosition;
            public Vector3 Angles => m_Angles;
            public MoveSettings Settings => m_Settings;
            public float MoveTime => m_MoveTime;
            public Ease MoveEase => m_MoveEase;
            public Ease RotateEase => m_RotateEase;
            [SerializeField, Header("RectTransformの場合、xとyの値の値が反映されます。")]
            private State m_State = State.Transform;
            public State GetState() => m_State;
        }

        private void Awake()
        {
            if (m_PlayOnAwake)
            {
                StartCoroutine(AllMove());
            }
        }

        /// <summary>
        /// オブジェクトを動かす処理
        /// </summary>
        /// <param name="moveIndex">動かすオブジェクトのデータIndex</param>
        public void ObjectMove(int moveIndex)
        {
            if (m_Movement.Length > moveIndex)
            {
                Movement movement = m_Movement[moveIndex];

                //移動の処理を始める
                switch (m_Movement[moveIndex].GetState())
                {
                    case State.Transform: TransformMove(movement); break;
                    case State.RectTransform: RectMove(movement); break;
                }
            }
            else
            {
                Debug.LogError($"Indexの値がLengthの値を上回っています！ Length{m_Movement.Length} : index{moveIndex}");
            }
        }


        /// <summary>
        /// 配列に入れてある処理を上から順に処理していく
        /// </summary>
        public IEnumerator AllMove()
        {
            yield return new WaitForSeconds(m_FirstWaitTime);

            for (int i = 0; i < m_Movement.Length; i++)
            {
                Debugger.Log("移動");
                ObjectMove(i);
                yield return new WaitForSeconds(m_Movement[i].MoveTime);
            }
        }

        /// <summary>
        /// Transform型のオブジェクトを動かす
        /// </summary>
        private void TransformMove(Movement move)
        {
            if (move.Settings != MoveSettings.Rotate)
                move.MoveObject.transform.TweenMove(move.EndPosition, move.MoveTime);
            if (move.Settings != MoveSettings.Move)
                move.MoveObject.transform.TweenRotate(move.Angles, move.MoveTime);
        }

        /// <summary>
        /// RectTransform型のオブジェクトを動かす
        /// </summary>
        private void RectMove(Movement move)
        {
            RectTransform rect = move.MoveObject.GetComponent<RectTransform>();

            if (move.Settings != MoveSettings.Rotate)
                rect.TweenAnchorMove(move.EndPosition, move.MoveTime);
            if (move.Settings != MoveSettings.Move)
                rect.TweenRotate(move.Angles, move.MoveTime);
        }
    }
}