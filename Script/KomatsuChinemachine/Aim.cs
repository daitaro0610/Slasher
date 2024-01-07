using UnityEngine;
using System;

namespace KD.Cinemachine
{
    [Serializable]
    public struct Aim
    {
        public Axis m_VerticalAxis;
        public Axis m_HorizontalAxis;

        [Serializable]
        public struct Axis
        {
            [SerializeField]
            private float m_Value;

            public ValueRange m_ValueRange;

            [Serializable]
            public struct ValueRange
            {
                public float Max;
                public float Min;
            }

            [SerializeField]
            private float m_Speed;
            [SerializeField]
            private float m_AccelTime;
            private float m_CurrentSpeed;
            public bool m_Invert;
            public float CurrentSpeed { set => m_CurrentSpeed = value; get => m_CurrentSpeed; }


            public float Value { set => m_Value = value; get => m_Value; }
            public float Speed { set => m_Speed = value; get => m_Speed; }
            public float AccelTime { set => m_AccelTime = value; get => m_AccelTime; }
        }
    }
}
