using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterAttack : MonoBehaviour
{
    private bool m_IsUpdateAttack = false; //Update�ŃA�^�b�N���Ăяo��������ۂ�True�ɂȂ�(�u���X�Ȃǂ̒����I�ȍU����)
    [SerializeField, Header("Update��Ԃ̎��ɉ���Ɉ��U���������s�킹��̂�")]
    private int m_AttackFrequencyInUpdates = 5;
    private int m_Counter; //FixedUpdate������s�������m�F�p

    [SerializeField, Header("�U����")]
    private int[] m_Damages;
    private int m_CurrentDamageValue;

    [SerializeField, Header("�U���p�̃R���C�_�[")]
    private Collider[] m_AttackColliders;
    private Collider m_CurrentCollider;

    [SerializeField]
    private LayerMask m_DetectionLayer = -1;

    private void Awake()
    {
        m_IsUpdateAttack = false;
        m_Counter = 0;

        //������Ԃł͂͗v�f��1�Ԗڂ��w�肷��
        m_CurrentCollider = m_AttackColliders[0];
        m_CurrentDamageValue = m_Damages[0];
    }

    private void FixedUpdate()
    {
        //�U����Ԃ̎��ȊO�͏������s��Ȃ�
        if (!m_IsUpdateAttack) return;

        m_Counter++;
        //���̉񐔑ҋ@������U���������s��
        if (m_AttackFrequencyInUpdates < m_Counter)
        {
            m_Counter = 0;
            Attack();
        }
    }

    /// <summary>
    /// �����I�ɍU���������s���ۂɌĂ�
    /// </summary>
    /// <param name="time">�U�������̎���</param>
    /// <returns></returns>
    public CharacterAttack UpdateAttackEnabled(bool isbool)
    {
        m_IsUpdateAttack = isbool;
        m_Counter = 0;
        return this;
    }

    /// <summary>
    /// �U���͂�ύX����
    /// </summary>
    /// <param name="index">m_Damages�̗v�f�ԍ�</param>
    /// <returns></returns>
    public CharacterAttack SetDamageValue(int index)
    {
        if (m_Damages.Length <= index) return this;
        m_CurrentDamageValue = m_Damages[index];
        return this;
    }

    public CharacterAttack SetCollider(int index)
    {
        if (m_AttackColliders.Length <= index) return this;

        m_CurrentCollider = m_AttackColliders[index];
        return this;
    }

    // Start is called before the first frame update
    public bool Attack()
    {
       return Attack(m_CurrentDamageValue);
    }

    public bool Attack(int damageValue)
    {
        Collider[] colliders = CheckHitCollider();

        if (colliders == null) return false;

        bool canAttack = false;
        List<GameObject> damageTargetList = new();

        damageValue = AttackPowerCalculator.Attack(damageValue);

        foreach (Collider collider in colliders)
        {
            //���蔲����R���C�_�[�͔��肵�Ȃ�
            if (collider.isTrigger) continue;

            IDamageble damageble = collider.GetComponent<IDamageble>();

            //�����I�u�W�F�N�g�̏ꍇ�͂����_���[�W�������s��Ȃ�
            if (damageTargetList.Contains(collider.transform.root.gameObject)) continue;

            //���g�̃R���C�_�[�͖�������
            if (damageble != null)
            {
                canAttack = true;
                damageble.ReceiveDamage(damageValue);
                damageTargetList.Add(collider.transform.root.gameObject);
            }
        }

        return canAttack;
    }

    private Collider[] CheckHitCollider()
    {
        //�U���̏������Ăт������^�C�~���O��m_AttackCollider�ɏՓ˂��Ă���ق��̃R���C�_�[���擾����
        //Box�R���C�_�[�̏ꍇ
        if (m_CurrentCollider is BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
            Vector3 halfExtents = boxCollider.size * 0.5f;
            Quaternion rotation = boxCollider.transform.rotation;
            return Physics.OverlapBox(center, halfExtents, rotation, m_DetectionLayer);
        }
        //�J�v�Z���R���C�_�[�̏ꍇ
        else if (m_CurrentCollider is CapsuleCollider capsuleCollider)
        {
            Vector3 point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + 0.5f * capsuleCollider.height * Vector3.up);
            Vector3 point2 = capsuleCollider.transform.TransformPoint(capsuleCollider.center - 0.5f * capsuleCollider.height * Vector3.up);
            float radius = capsuleCollider.radius;
            return Physics.OverlapCapsule(point1, point2, radius, m_DetectionLayer);
        }
        //�X�t�B�A�R���C�_�[�̏ꍇ
        else if (m_CurrentCollider is SphereCollider sphereCollider)
        {
            Vector3 center = sphereCollider.transform.TransformPoint(sphereCollider.center);
            float radius = sphereCollider.radius;
            return Physics.OverlapSphere(center, radius, m_DetectionLayer);
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (m_CurrentCollider is BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
            Vector3 halfExtents = boxCollider.size * 0.5f;
            Quaternion rotation = boxCollider.transform.rotation;
            Gizmos.DrawCube(center, boxCollider.size);
        }
        //�X�t�B�A�R���C�_�[�̏ꍇ
        else if (m_CurrentCollider is SphereCollider sphereCollider)
        {
            Vector3 center = sphereCollider.transform.TransformPoint(sphereCollider.center);
            float radius = sphereCollider.radius;
            Gizmos.DrawSphere(center, radius);
        }
        else if(m_CurrentCollider is CapsuleCollider capsuleCollider)
        {
            Vector3 direction = Vector3.zero;
            switch (capsuleCollider.direction)
            {
                case 0:
                    //X���Ɍ����Ă���
                    direction = Vector3.right;
                    break;
                case 1:
                    //Y���Ɍ����Ă���
                    direction = Vector3.up;
                    break;
                case 2:
                    //Z���Ɍ����Ă���
                    direction = Vector3.forward;
                    break;
            }

            Vector3 point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + 0.5f * capsuleCollider.height * direction);
            Vector3 point2 = capsuleCollider.transform.TransformPoint(capsuleCollider.center - 0.5f * capsuleCollider.height * direction);
            float radius = capsuleCollider.radius;
            Gizmos.DrawSphere(point1, radius);
            Gizmos.DrawSphere(point2, radius);
        }
    }
}
