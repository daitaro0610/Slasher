using UnityEngine;
using System;

namespace KD.Cinemachine
{
    [Serializable]
    public struct Body
    {
        [SerializeField]
        private float m_MaxDistance;
        [SerializeField]
        private Vector3 m_Offset;

        public float MaxDistance { set { m_MaxDistance = value; } get => m_MaxDistance; }
        public Vector3 Offset { set { m_Offset = value; } get => m_Offset; }
    }
}