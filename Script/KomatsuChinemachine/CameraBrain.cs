using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KD.Cinemachine
{

    [ExecuteInEditMode]//Awake等がUnityエディタを再生していなくても実行されるように
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
        /// シーン上にあるVirtualCameraを追加していく
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
        /// 削除されたVirtualCameraをリストから消していく
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
        /// Priorityが変更されたときに呼び出す
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

                    //パラメータを設定する
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
        /// バーチャルカメラを作成する
        /// </summary>
        public void CreateVirtualCamera()
        {
            float far = m_MainCam.farClipPlane;
            float near = m_MainCam.nearClipPlane;
            float fov = m_MainCam.fieldOfView;


            GameObject virtualCameraObj = new("VirtualCamera");

            VirtualCamera vc = virtualCameraObj.AddComponent<VirtualCamera>();

            //Priorityの値は、すべてのVirtualCameraのうち最大の値+1
            int maxPriority = 0;
            if (m_VirtualCameras.Count != 0)
            {
                maxPriority = m_VirtualCameras.Max(v => v.Priority);
            }
            vc.Priority = maxPriority + 1;

            //VirtualCameraの値をVCSettingsの値に応じて初期化
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