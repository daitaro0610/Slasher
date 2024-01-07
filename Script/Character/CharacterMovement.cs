using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KD.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float[] m_MoveSpeedList;
    private float m_MoveSpeed;
    [SerializeField]
    private float m_RotationSpeed;
    [SerializeField]
    private float m_JumpPower;
    [SerializeField]
    private float m_AvoidancePower;

    private Rigidbody rb;

    [SerializeField]
    private float m_Gravity = 9.8f;

    [SerializeField]
    private Vector3 m_GroundRayOffset = Vector3.up * 0.1f;
    [SerializeField]
    private LayerMask m_GroundLayer;

    private Quaternion m_TargetRotation;

    private Transform m_MainCameraTransform;

    public enum State
    {
        Move,
        Stop,
    }
    [SerializeField]
    private State m_MovementState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        m_MainCameraTransform = Camera.main.transform;
        m_MoveSpeed = m_MoveSpeedList[0];
    }

    public void MoveForward()
    {
        //�v���C���[��O�Ɉړ�������
        rb.MovePosition(transform.position + (m_MoveSpeed * Time.fixedDeltaTime * transform.forward));
    }

    public void OnCharacterMove(Vector2 velocity)
    {
        if (m_MovementState == State.Stop) return;

        //TODO �����Ȍ��ԂȂǂ����蔲���Ă��܂��̂ŉ����􋁃�

        //�O�i
        MoveForward();

        Vector3 movement = new Vector3(velocity.x, 0, velocity.y);
        //�J�����̕��������琳�ʂ��v�Z����
        Quaternion horizontalRotation = Quaternion.AngleAxis(m_MainCameraTransform.eulerAngles.y, Vector3.up);
        //��������]������
        LookTargetRotation(horizontalRotation, movement);
    }

    /// <summary>
    /// �W�����v������
    /// </summary>
    public void Jump()
    {
        rb.AddForce(Vector3.up * m_JumpPower, ForceMode.Impulse);
    }

    public void Avoidance(Vector3 velocity)
    {
        rb.AddForce(velocity* m_AvoidancePower, ForceMode.VelocityChange);
    }


    public void OnGravity()
    {
        var offsetPosition = transform.position + m_GroundRayOffset;
        //�n�ʂ̃��C�ɏՓ˂����ꍇ�͗����������s���B����ȉ��ɒn�ʂ��Ȃ��ꍇ�͗�����������߂�
        RaycastHit[] hits = Physics.RaycastAll(offsetPosition, Vector3.down, Mathf.Infinity, m_GroundLayer);

        if (hits.Length > 0)
        {
            // �J�X�^���̏d�͂�K�p
            Vector3 gravityForce = Vector3.up * -m_Gravity;
            rb.AddForce(gravityForce, ForceMode.Acceleration);
        }
    }

    private void LookTargetRotation(Quaternion horizontalRotation, Vector3 velocity)
    {
        Vector3 rotationVelocity = horizontalRotation * velocity.normalized;
        float rotationSpeed = m_RotationSpeed * Time.deltaTime;
        if (rotationVelocity.magnitude > 0.5f)
            m_TargetRotation = Quaternion.LookRotation(rotationVelocity, Vector3.up);

        //���X�ɉ�]������
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation, rotationSpeed);
    }

    /// <summary>
    /// ��Ԃ̕ύX�@Stop�ɂ����ꍇ�͈ړ��̏����Ȃǂœ����Ȃ��Ȃ�
    /// </summary>
    /// <param name="state"></param>
    public void SetState(State state) => m_MovementState = state;

    /// <summary>
    /// �ړ����x��ύX����
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(int index)
    {
        if (m_MoveSpeedList.Length > index)
        {
            m_MoveSpeed = m_MoveSpeedList[index];
        }
        else
        {
            m_MoveSpeed = m_MoveSpeedList[0];
        }
    }

    public float GetSpeed() => m_MoveSpeed;

}
