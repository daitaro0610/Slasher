using UnityEngine;

namespace KD.Cinemachine
{

    [ExecuteInEditMode]//Awake等がUnityエディタを再生していなくても実行されるように
    public class VirtualCameraCollider : MonoBehaviour
    {
        [SerializeField, Min(0.01f), Tooltip("カメラ自体の衝突判定の半径")]
        private float m_Radius = 0.5f;
        [SerializeField]
        private bool m_IsCollisionEnable;

        [SerializeField, Min(0), Tooltip("カメラからターゲットへの視線が遮蔽されていないか\n確認するために使うレイキャストの距離")]
        private float m_DistanceLimit = 0;

        private VirtualCamera m_VirtualCamera;

        [SerializeField]
        private LayerMask m_LayerMask;

        [HideInInspector]
        public string m_IgnoreTag = "";

        private float m_TargetToDistance;
        private float m_LerpedDistance;

        [SerializeField, Min(0), Tooltip("ターゲットが遮蔽物に隠れてから\n実際に障害物回避が始まるまでの時間")]
        private float m_MinimumOcclusionTime;
        private float m_OcclusionTimer;

        [SerializeField, Range(0, 2), Tooltip("障害物回避でターゲットに近づいたときの\n状態を維持する秒数")]
        private float m_SmoothingTime = 0;
        private float m_ElapsedTime = 0f;

        [SerializeField, Range(0, 10), Tooltip("遮蔽物が無くなってからターゲットに戻るときの素早さ")]
        private float m_Damping;
        private float m_DampingTime;

        [SerializeField, Range(0, 10), Tooltip("ターゲットが遮蔽物に隠れてから障害物回避するときの素早さ")]
        private float m_DampingWhenOccluded;
        private float m_DampingWhenOccludedTime;

        private void Awake()
        {
            ResetParam();
        }

        private void OnEnable()
        {
            ResetParam();
        }

        private void ResetParam()
        {
            m_LerpedDistance = 0;
            m_DampingTime = 0;
            m_DampingWhenOccludedTime = 0;
            m_OcclusionTimer = m_MinimumOcclusionTime;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!m_VirtualCamera)
            {
                m_VirtualCamera = GetComponent<VirtualCamera>();
            }

            if (!m_VirtualCamera.GetTarget()) return;

            // ターゲットからカメラへのベクトルを計算
            Vector3 cameraToTarget = transform.position - (m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset);

            // スフィアキャストを実行
            RaycastHit[] hits = Physics.SphereCastAll(m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset, m_Radius, cameraToTarget.normalized, m_VirtualCamera.Body.MaxDistance, m_LayerMask);
            m_TargetToDistance = 0;
            foreach (RaycastHit h in hits)
            {
                //また、ターゲット自身のタグもIgnoreとする
                //if (h.collider.CompareTag(m_VirtualCamera.GetTarget().tag)) continue;

                //判定したくないタグの場合は無視する。
                if (m_IgnoreTag != "" && h.collider.CompareTag(m_IgnoreTag)) continue;


                // スフィアキャストが当たった場合、カメラを当たった位置に近づける
                float distanceToHit = h.distance;

                if (m_DistanceLimit > distanceToHit) continue;

                m_TargetToDistance = Mathf.Max(m_TargetToDistance, m_VirtualCamera.Body.MaxDistance - distanceToHit);
            }

            //何かに当たってカメラとの距離を縮める場合の処理
            if (m_TargetToDistance != 0)
            {
                Debug.DrawRay(m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset, cameraToTarget, Color.red);

                if (m_OcclusionTimer > 0)
                {
                    m_OcclusionTimer -= Time.deltaTime;
                    //TargetToDistanceの値を反映させない
                    return;
                }

                if (m_DampingTime < m_Damping)
                {
                    m_DampingTime += Time.deltaTime;
                }
                else m_DampingTime = m_Damping;

                SetDistance(m_DampingTime, m_Damping);
                //障害物回避を解除するまでの時間をリセットする
                ResetSmoothTime();
                ResetDampingWhenOccludedTime();
            }
            else
            {
                Debug.DrawRay(m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset, cameraToTarget);
                if (m_ElapsedTime > 0)
                {
                    m_ElapsedTime -= Time.deltaTime;
                    return;
                }

                if (m_DampingWhenOccludedTime < m_DampingWhenOccluded)
                {
                    m_DampingWhenOccludedTime += Time.deltaTime;
                }
                else m_DampingWhenOccludedTime = m_DampingWhenOccluded;

                //何にも当たらない場合は、TargetToDistanceは0にして最大の距離までカメラを遠ざける
                SetDistance(m_DampingWhenOccludedTime, m_DampingWhenOccluded);

                //ターゲットが遮蔽物に隠れてから実際に障害物回避が始まるまでの時間を初期化する
                ResetOcclusionTime();
                ResetDampingTime();
            }
        }

        private void SetDistance(float time, float maxTime)
        {
            if (Application.isPlaying)
            {
                // Unity再生中の処理
                if (maxTime != 0)
                {
                    m_LerpedDistance = Mathf.Lerp(m_LerpedDistance, m_TargetToDistance, time / maxTime);
                    m_VirtualCamera.TargetToDistance = m_LerpedDistance;
                }
                else m_VirtualCamera.TargetToDistance = m_TargetToDistance;
            }
            else
            {
                // エディターモードでの処理
                m_VirtualCamera.TargetToDistance = m_TargetToDistance;
            }
        }


        private void ResetSmoothTime()
          => m_ElapsedTime = m_SmoothingTime;

        private void ResetOcclusionTime()
          => m_OcclusionTimer = m_MinimumOcclusionTime;

        private void ResetDampingTime()
            => m_DampingTime = 0;

        private void ResetDampingWhenOccludedTime()
            => m_DampingWhenOccludedTime = 0;

        public void TagClear()
        {
            m_IgnoreTag = "";
        }

        private void OnDrawGizmos()
        {
            if (m_IsCollisionEnable)
                Gizmos.DrawWireSphere(transform.position, m_Radius);
        }
    }
}