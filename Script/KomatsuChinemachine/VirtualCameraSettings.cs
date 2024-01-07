using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.Cinemachine
{
    [CreateAssetMenu(fileName = "VirtualCameraSettings", menuName = "CreateData/VirtualCameraSettings")]
    public class VirtualCameraSettings : ScriptableObject
    {
        [SerializeField]
        private Body m_Body;
        [SerializeField]
        private Aim m_Aim;

        public Aim Aim => m_Aim;
        public Body Body => m_Body;
    }
}