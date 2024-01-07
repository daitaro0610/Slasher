using UnityEngine;

namespace KD.Cinemachine
{

    [ExecuteInEditMode]//Awake����Unity�G�f�B�^���Đ����Ă��Ȃ��Ă����s�����悤��
    public class VirtualCameraCollider : MonoBehaviour
    {
        [SerializeField, Min(0.01f), Tooltip("�J�������̂̏Փ˔���̔��a")]
        private float m_Radius = 0.5f;
        [SerializeField]
        private bool m_IsCollisionEnable;

        [SerializeField, Min(0), Tooltip("�J��������^�[�Q�b�g�ւ̎������Օ�����Ă��Ȃ���\n�m�F���邽�߂Ɏg�����C�L���X�g�̋���")]
        private float m_DistanceLimit = 0;

        private VirtualCamera m_VirtualCamera;

        [SerializeField]
        private LayerMask m_LayerMask;

        [HideInInspector]
        public string m_IgnoreTag = "";

        private float m_TargetToDistance;
        private float m_LerpedDistance;

        [SerializeField, Min(0), Tooltip("�^�[�Q�b�g���Օ����ɉB��Ă���\n���ۂɏ�Q��������n�܂�܂ł̎���")]
        private float m_MinimumOcclusionTime;
        private float m_OcclusionTimer;

        [SerializeField, Range(0, 2), Tooltip("��Q������Ń^�[�Q�b�g�ɋ߂Â����Ƃ���\n��Ԃ��ێ�����b��")]
        private float m_SmoothingTime = 0;
        private float m_ElapsedTime = 0f;

        [SerializeField, Range(0, 10), Tooltip("�Օ����������Ȃ��Ă���^�[�Q�b�g�ɖ߂�Ƃ��̑f����")]
        private float m_Damping;
        private float m_DampingTime;

        [SerializeField, Range(0, 10), Tooltip("�^�[�Q�b�g���Օ����ɉB��Ă����Q���������Ƃ��̑f����")]
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

            // �^�[�Q�b�g����J�����ւ̃x�N�g�����v�Z
            Vector3 cameraToTarget = transform.position - (m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset);

            // �X�t�B�A�L���X�g�����s
            RaycastHit[] hits = Physics.SphereCastAll(m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset, m_Radius, cameraToTarget.normalized, m_VirtualCamera.Body.MaxDistance, m_LayerMask);
            m_TargetToDistance = 0;
            foreach (RaycastHit h in hits)
            {
                //�܂��A�^�[�Q�b�g���g�̃^�O��Ignore�Ƃ���
                //if (h.collider.CompareTag(m_VirtualCamera.GetTarget().tag)) continue;

                //���肵�����Ȃ��^�O�̏ꍇ�͖�������B
                if (m_IgnoreTag != "" && h.collider.CompareTag(m_IgnoreTag)) continue;


                // �X�t�B�A�L���X�g�����������ꍇ�A�J�����𓖂������ʒu�ɋ߂Â���
                float distanceToHit = h.distance;

                if (m_DistanceLimit > distanceToHit) continue;

                m_TargetToDistance = Mathf.Max(m_TargetToDistance, m_VirtualCamera.Body.MaxDistance - distanceToHit);
            }

            //�����ɓ������ăJ�����Ƃ̋������k�߂�ꍇ�̏���
            if (m_TargetToDistance != 0)
            {
                Debug.DrawRay(m_VirtualCamera.GetTarget().position + m_VirtualCamera.Body.Offset, cameraToTarget, Color.red);

                if (m_OcclusionTimer > 0)
                {
                    m_OcclusionTimer -= Time.deltaTime;
                    //TargetToDistance�̒l�𔽉f�����Ȃ�
                    return;
                }

                if (m_DampingTime < m_Damping)
                {
                    m_DampingTime += Time.deltaTime;
                }
                else m_DampingTime = m_Damping;

                SetDistance(m_DampingTime, m_Damping);
                //��Q���������������܂ł̎��Ԃ����Z�b�g����
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

                //���ɂ�������Ȃ��ꍇ�́ATargetToDistance��0�ɂ��čő�̋����܂ŃJ��������������
                SetDistance(m_DampingWhenOccludedTime, m_DampingWhenOccluded);

                //�^�[�Q�b�g���Օ����ɉB��Ă�����ۂɏ�Q��������n�܂�܂ł̎��Ԃ�����������
                ResetOcclusionTime();
                ResetDampingTime();
            }
        }

        private void SetDistance(float time, float maxTime)
        {
            if (Application.isPlaying)
            {
                // Unity�Đ����̏���
                if (maxTime != 0)
                {
                    m_LerpedDistance = Mathf.Lerp(m_LerpedDistance, m_TargetToDistance, time / maxTime);
                    m_VirtualCamera.TargetToDistance = m_LerpedDistance;
                }
                else m_VirtualCamera.TargetToDistance = m_TargetToDistance;
            }
            else
            {
                // �G�f�B�^�[���[�h�ł̏���
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