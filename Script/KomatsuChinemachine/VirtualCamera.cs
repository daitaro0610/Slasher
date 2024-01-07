using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KD.Tweening;

namespace KD.Cinemachine
{
    [ExecuteInEditMode]//Awake等がUnityエディタを再生していなくても実行されるように
    public class VirtualCamera : MonoBehaviour
    {
        private CameraBrain m_Brain;
        private bool m_IsActive = true;

        [SerializeField]
        private int m_Priority = 0; //VirtualCameraの優先度
        private int m_MemoryPriority;
        public int Priority { set { m_Priority = value; } get => m_Priority; }

        [SerializeField]
        private Transform m_Target; // カメラが追跡するターゲット
        public Transform GetTarget() => m_Target;


        private enum State
        {
            Follow,
            LookAt,
        }
        [SerializeField]
        private State m_State = State.Follow;


        [SerializeField, Min(0.01f)]
        private float m_FOV;
        [SerializeField, Min(0.01f)]
        private float m_Near;
        [SerializeField, Min(0.11f)]
        private float m_Far;

        [SerializeField]
        private bool m_IsControll = true;

        [SerializeField,Min(0.01f)]
        private float m_Intensity = 1f;

        public float FOV
        {
            set
            {
                if (m_FOV != value)
                {
                    m_FOV = value;
                    FOVChanged();
                }
            }
            get => m_FOV;
        }

        public float Near
        {
            set
            {
                if (m_Near != value)
                {
                    m_Near = value;
                    NearChanged();
                }
            }
            get => m_Near;
        }

        public float Far
        {
            set
            {
                if (m_Far != value)
                {
                    m_Far = value;
                    FarChanged();
                }
            }
            get => m_Far;
        }

        [SerializeField,HideInInspector]
        private VirtualCameraSettings m_Settings;
        public VirtualCameraSettings Settings
        {
            get
            {
                return m_Settings;
            }
            set
            {
                m_Settings = value;
            }
        }

        [Space(10)]
        public Body Body;
        public Aim Aim;

        private float m_TargetToDistance;
        public float TargetToDistance { set { m_TargetToDistance = value; } get => m_TargetToDistance; }

        private PlayerGame m_PlayerGame;

        private Vector2 m_CameraRotateVelocity;

        private void Awake()
        {
            m_Brain = FindObjectOfType<CameraBrain>();
            m_Brain.AddVirtualCamera(this);

            m_MemoryPriority = m_Priority;
            m_TargetToDistance = 0;

            m_PlayerGame = new();

            m_PlayerGame.Camera.CameraRotate.started += OnCameraRotate;
            m_PlayerGame.Camera.CameraRotate.performed += OnCameraRotate;
            m_PlayerGame.Camera.CameraRotate.canceled += OnCameraRotate;

            m_PlayerGame.Enable();

            Body = m_Settings.Body;
            Aim = m_Settings.Aim;
        }

        private void Update()
        {
            CheckPriority();
            if (!m_IsActive || Time.timeScale == 0) return;

            if (m_State == State.Follow)
            {
                //回転の処理
                FollowRotation();
            }

            if (!m_Target) return;
            //ターゲットの方向を見る
            LookTarget();
        }

        private void OnValidate()
        {
            if (!m_Brain)
            {
                m_Brain = FindAnyObjectByType<CameraBrain>();
                m_Brain.AddVirtualCamera(this);
            }

            m_TargetToDistance = 0;

            FOVChanged();
            NearChanged();
            FarChanged();
        }

        public void SetParameter()
        {
            Body = m_Settings.Body;
            Aim = m_Settings.Aim;
        }

        public void SetBrain(CameraBrain cameraBrain)
        {
            m_Brain = cameraBrain;
        }

        private void FOVChanged()
        {
            if (!m_IsActive) return;
            m_Brain.SetFOV();
        }

        private void NearChanged()
        {
            if (!m_IsActive) return;
            m_Brain.SetNear();
        }

        private void FarChanged()
        {
            if (!m_IsActive) return;
            m_Brain.SetFar();
        }

        /// <summary>
        /// Priorityの値が変更されたかどうか
        /// </summary>
        private void CheckPriority()
        {
            if (m_MemoryPriority == m_Priority) return;

            m_MemoryPriority = m_Priority;
            m_Brain.ChangedPriority();
        }

        private void OnDestroy()
        {
            m_Brain.RemoveVirtualCamera(this);
        }

        public void SetActivePriority(bool isbool) => m_IsActive = isbool;
        public void SetIntensity(float value) => m_Intensity = value;


        private Vector2 Input()
        {
            // マウスの入力を取得
            return m_CameraRotateVelocity;
        }

        public void OnCameraRotate(InputAction.CallbackContext callback)
        {
            if (m_IsControll)
                m_CameraRotateVelocity = callback.ReadValue<Vector2>() * m_Intensity;
        }

        /// <summary>
        /// 回転の処理
        /// AimクラスのValueで回転の値を変更可能
        /// </summary>
        private void FollowRotation()
        {
            var input = Input();

            //回転速度を徐々に早める
            //水平方向
            float horizontalTargetRotation = Aim.m_HorizontalAxis.Speed * input.x;
            float horizontalRotation = Mathf.MoveTowards(Aim.m_HorizontalAxis.CurrentSpeed, horizontalTargetRotation, Aim.m_HorizontalAxis.AccelTime * Time.deltaTime);
            Aim.m_HorizontalAxis.CurrentSpeed = horizontalRotation;

            //垂直方向
            float verticalTargetRotation = Aim.m_HorizontalAxis.Speed * input.y;
            float verticalRotation = Mathf.MoveTowards(Aim.m_VerticalAxis.CurrentSpeed, verticalTargetRotation, Aim.m_VerticalAxis.AccelTime * Time.deltaTime);
            Aim.m_VerticalAxis.CurrentSpeed = verticalRotation;

            if (Aim.m_HorizontalAxis.m_Invert)
                horizontalRotation *= -1;

            if (Aim.m_VerticalAxis.m_Invert)
                verticalRotation *= -1;

            // 水平方向の回転
            Aim.m_HorizontalAxis.Value += horizontalRotation;
            //180から-180までの値でループするようにする
            Aim.m_HorizontalAxis.Value = Mathf.Repeat(Aim.m_HorizontalAxis.Value + 180, 360) - 180;
            // 垂直方向の回転角度を更新し、制限を適用
            Aim.m_VerticalAxis.Value -= verticalRotation;
            Aim.m_VerticalAxis.Value = Mathf.Clamp(Aim.m_VerticalAxis.Value, Aim.m_VerticalAxis.m_ValueRange.Min, Aim.m_VerticalAxis.m_ValueRange.Max);
        }


        /// <summary>
        /// ターゲットの方向を向く
        /// </summary>
        private void LookTarget()
        {
            // ターゲットの位置を取得
            Vector3 targetPosition = m_Target.position;
            // カメラの位置を更新
            Vector3 cameraPosition = targetPosition;
            cameraPosition += Quaternion.Euler(Aim.m_VerticalAxis.Value, Aim.m_HorizontalAxis.Value, 0f) * new Vector3(0f, 0f, -(Body.MaxDistance - m_TargetToDistance));

            if (m_State == State.Follow)
            {
                transform.position = cameraPosition + Body.Offset;
            }

            // カメラをターゲットに向ける
            transform.LookAt(targetPosition + Body.Offset);
        }

        /// <summary>
        /// カメラを振動させる
        /// </summary>
        public void Shake(float strength, float vibrato, float duration)
        {
            //カメラを揺らす
            KDTweener.ShakeTo(
                () => Body.Offset,
                (x) => Body.Offset = x,
                strength,
                vibrato,
                duration)
                .SetOption(new Tweening.Plugin.Options.ShakeOptions
                {
                    axisConstraint = AxisConstraint.XY,
                    fadeOut = true
                });
        }
    }
}