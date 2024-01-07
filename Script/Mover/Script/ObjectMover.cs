using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening;

namespace KD.Tweening.Mover
{
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField, Header("�ŏ��ɂ��ׂĂ̏��������s���邩�ǂ���")]
        private bool m_PlayOnAwake = false;
        [SerializeField, Header("PlayOnAwake��On�ɂ����Ƃ��̑҂�����")]
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

            [SerializeField, Tooltip("�I�_")]
            private Vector3 m_EndPosition;
            [SerializeField, Tooltip("�p�x")]
            private Vector3 m_Angles;

            [SerializeField, Tooltip("��]�������邩�����Ȃ���")]
            private MoveSettings m_Settings = MoveSettings.MoveAndRotate;
            [SerializeField, Tooltip("�ړ����鎞��")]
            private float m_MoveTime;

            [SerializeField, Header("�ړ����̈ړ���Ease")]
            private Ease m_MoveEase = Ease.Linear;
            [SerializeField, Header("��]���̈ړ���Ease")]
            private Ease m_RotateEase = Ease.Linear;

            public Vector3 EndPosition => m_EndPosition;
            public Vector3 Angles => m_Angles;
            public MoveSettings Settings => m_Settings;
            public float MoveTime => m_MoveTime;
            public Ease MoveEase => m_MoveEase;
            public Ease RotateEase => m_RotateEase;
            [SerializeField, Header("RectTransform�̏ꍇ�Ax��y�̒l�̒l�����f����܂��B")]
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
        /// �I�u�W�F�N�g�𓮂�������
        /// </summary>
        /// <param name="moveIndex">�������I�u�W�F�N�g�̃f�[�^Index</param>
        public void ObjectMove(int moveIndex)
        {
            if (m_Movement.Length > moveIndex)
            {
                Movement movement = m_Movement[moveIndex];

                //�ړ��̏������n�߂�
                switch (m_Movement[moveIndex].GetState())
                {
                    case State.Transform: TransformMove(movement); break;
                    case State.RectTransform: RectMove(movement); break;
                }
            }
            else
            {
                Debug.LogError($"Index�̒l��Length�̒l�������Ă��܂��I Length{m_Movement.Length} : index{moveIndex}");
            }
        }


        /// <summary>
        /// �z��ɓ���Ă��鏈�����ォ�珇�ɏ������Ă���
        /// </summary>
        public IEnumerator AllMove()
        {
            yield return new WaitForSeconds(m_FirstWaitTime);

            for (int i = 0; i < m_Movement.Length; i++)
            {
                Debugger.Log("�ړ�");
                ObjectMove(i);
                yield return new WaitForSeconds(m_Movement[i].MoveTime);
            }
        }

        /// <summary>
        /// Transform�^�̃I�u�W�F�N�g�𓮂���
        /// </summary>
        private void TransformMove(Movement move)
        {
            if (move.Settings != MoveSettings.Rotate)
                move.MoveObject.transform.TweenMove(move.EndPosition, move.MoveTime);
            if (move.Settings != MoveSettings.Move)
                move.MoveObject.transform.TweenRotate(move.Angles, move.MoveTime);
        }

        /// <summary>
        /// RectTransform�^�̃I�u�W�F�N�g�𓮂���
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