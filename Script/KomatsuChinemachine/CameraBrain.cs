using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KD.Cinemachine
{

    [ExecuteInEditMode]//Awake����Unity�G�f�B�^���Đ����Ă��Ȃ��Ă����s�����悤��
    public class CameraBrain : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private Camera m_MainCam;

        private List<VirtualCamera> m_VirtualCameras = new();

        [SerializeField,ReadOnly]
        private VirtualCamera m_CurrentVirtualCamera;

        [SerializeField, ReadOnly]
        private VirtualCameraSettings m_VirtualCameraSettings;

        private void Awake()
        {
          //  m_VirtualCameras = new();
        }

        private void Update()
        {
            if (m_MainCam == null)
            {
                m_MainCam = Camera.main;
            }
        }

        private void LateUpdate()
        {
            transform.position = m_CurrentVirtualCamera.transform.position;
            transform.eulerAngles = m_CurrentVirtualCamera.transform.eulerAngles;
        }

        /// <summary>
        /// �V�[����ɂ���VirtualCamera��ǉ����Ă���
        /// </summary>
        /// <param name="vc"></param>
        public void AddVirtualCamera(VirtualCamera vc)
        {
            if (m_VirtualCameras == null)
                m_VirtualCameras = new();

            if (!m_VirtualCameras.Contains(vc))
            {
                m_VirtualCameras.Add(vc);
                ChangedPriority();
            }
        }

        /// <summary>
        /// �폜���ꂽVirtualCamera�����X�g��������Ă���
        /// </summary>
        /// <param name="vc"></param>
        public void RemoveVirtualCamera(VirtualCamera vc)
        {
            if (m_VirtualCameras.Contains(vc))
            {
                m_VirtualCameras.Remove(vc);
                ChangedPriority();
            }
        }

        /// <summary>
        /// Priority���ύX���ꂽ�Ƃ��ɌĂяo��
        /// </summary>
        public void ChangedPriority()
        {
            if (m_VirtualCameras.Count == 0 || m_MainCam == null) return;

            int maxPriority = m_VirtualCameras.Max(v => v.Priority);
            foreach (VirtualCamera vc in m_VirtualCameras)
            {
                if (vc.Priority == maxPriority)
                {
                    if (m_CurrentVirtualCamera == vc) continue;

                    //�p�����[�^��ݒ肷��
                    m_CurrentVirtualCamera = vc;
                    vc.SetActivePriority(true);
                    SetFOV();
                    SetNear();
                    SetFar();
                }
                else
                {
                    vc.SetActivePriority(false);
                }
            }
        }

        /// <summary>
        /// �o�[�`�����J�������쐬����
        /// </summary>
        public void CreateVirtualCamera()
        {
            float far = m_MainCam.farClipPlane;
            float near = m_MainCam.nearClipPlane;
            float fov = m_MainCam.fieldOfView;


            GameObject virtualCameraObj = new("VirtualCamera");

            VirtualCamera vc = virtualCameraObj.AddComponent<VirtualCamera>();

            //Priority�̒l�́A���ׂĂ�VirtualCamera�̂����ő�̒l+1
            int maxPriority = 0;
            if (m_VirtualCameras.Count != 0)
            {
                maxPriority = m_VirtualCameras.Max(v => v.Priority);
            }
            vc.Priority = maxPriority + 1;

            //VirtualCamera�̒l��VCSettings�̒l�ɉ����ď�����
            vc.Far = far;
            vc.Near = near;
            vc.FOV = fov;

            vc.Settings = m_VirtualCameraSettings;
        }


        public void SetFOV()
          => m_MainCam.fieldOfView = m_CurrentVirtualCamera.FOV;

        public void SetNear()
          => m_MainCam.nearClipPlane = m_CurrentVirtualCamera.Near;

        public void SetFar()
          => m_MainCam.farClipPlane = m_CurrentVirtualCamera.Far;
    }
}