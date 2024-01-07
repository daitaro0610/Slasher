using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected CharacterAttack m_Attack;     //�U������
    protected AnimationController m_Animation; //�A�j���[�V����
    protected EnemyColliderDisabler m_ColliderDisabler;

    protected NavMeshAgent m_Agent; //�G�̈ړ������邽�߂�AI

    protected GameObject m_TargetObject;
    [SerializeField]
    protected LayerMask m_DitectionLayer;
    [SerializeField]
    protected Vector3 m_SearchRayOffset = Vector3.up;
    [SerializeField]
    protected Vector3 m_TargetRayOffset = Vector3.up;
    [SerializeField]
    protected float m_SearchAngle = 60f;
    protected bool m_IsPlayerFound;

    protected bool m_IsAngry;
    protected bool m_IsDeath;

    protected bool m_IsDazzling; //ῂ������ǂ���
    private int m_MemoryMoveNumber = 0; //�������Ɉړ��̃p�����[�^���L�����Ă������߂̕ϐ�

    private EnemyAllHuntCheck m_Check;


    protected void Awake()
    {
        m_Attack = GetComponent<CharacterAttack>();
        m_Animation = new AnimationController(gameObject);
        m_ColliderDisabler = GetComponent<EnemyColliderDisabler>();

        m_Agent = GetComponent<NavMeshAgent>();
        m_IsPlayerFound = false;
        m_IsAngry = false;
        m_IsDazzling = false;

        m_Check = FindAnyObjectByType<EnemyAllHuntCheck>();
        //�U���p�̃`�P�b�g���쐬����N���X
        new TicketedAttackManager();
    }

    /// <summary>
    /// �v���C���[��T������
    /// ��������True
    /// ������Ȃ��ꍇ��False
    /// </summary>
    /// <returns></returns>
    public virtual bool SearchPlayer()
    {
        return m_IsPlayerFound;
    }

    /// <summary>
    /// �v���C���[���͈͓��ɂ���ꍇ�͔�������
    /// </summary>
    /// <param name="player"></param>
    protected void PlayerInRange(Collider player)
    {
        Vector3 thisPos = transform.position + m_SearchRayOffset;
        Vector3 posDelta = player.transform.position + m_TargetRayOffset - thisPos;
        float targetAngle = Vector3.Angle(transform.forward, posDelta);

        if (targetAngle < m_SearchAngle)//����͈͓̔��ɂ����ꍇ
        {
            //�ǂȂǂ��ז����Ă���ꍇ�̓X���[����
            if (Physics.Raycast(thisPos, posDelta, out RaycastHit hit, Mathf.Infinity, m_DitectionLayer))
            {
                Debugger.Log(hit.collider.gameObject.name);
                Debug.DrawLine(thisPos, hit.point);
                if (hit.collider.CompareTag(TagName.Player))
                {
                    m_TargetObject = player.gameObject;
                    m_IsPlayerFound = true;
                }
            }
        }
    }

    public virtual GameObject GetTarget()
    {
        if (m_TargetObject == null) return null;

        return m_TargetObject;

    }

    public float CheckDistance()
    {
        return Vector3.Distance(m_TargetObject.transform.position, transform.position);
    }

    public abstract void Angry();

    public virtual bool CheckAngry()
        => m_IsAngry;

    public virtual bool CheckDeath()
        => m_IsDeath;

    public virtual void Roar()
        => m_Animation.Roar();

    public virtual void AngryRoar()
        => m_Animation.AngryRoar();

    public virtual bool CheckDazzling()
        => m_IsDazzling;

    public virtual AnimationController GetAnimation()
        => m_Animation;

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    public abstract void Attack();
    /// <summary>
    /// �͈͍U��
    /// </summary>
    public abstract void StrongAttack();

    public abstract void SpecialAttack();

    public abstract void Damage();

    /// <summary>
    /// �M���ʂ����ʂɂ��邩�ǂ���
    /// </summary>
    /// <param name="flashPos"></param>
    public void CheckFlashInFront(Vector3 flashPos)
    {
        Vector3 toPos = flashPos - transform.position;
        float flashAngle = Vector3.Angle(transform.forward, toPos);

        if (flashAngle < m_SearchAngle)//����͈͓̔��ɂ����ꍇ
        {
            Dizzy();
        }
    }

    /// <summary>
    /// �������̏���
    /// </summary>
    protected virtual void Dizzy()
    {
        Debugger.Log("����");
        m_Animation.Hit();
        m_Animation.SetMove(0);
        m_MemoryMoveNumber = m_Animation.GetMoveNumberInteger();
        m_IsDazzling = true;
        m_Agent.isStopped = true;
    }

    /// <summary>
    /// �����I�����̏���
    /// </summary>
    public virtual void DizzyEnd()
    {
        m_IsDazzling = false;
        m_Agent.isStopped = false;
        //�L�����Ă����ړ��̃p�����[�^�ɂ���
        m_Animation.SetMove(m_MemoryMoveNumber);
    }

    public virtual void Death()
    {
        m_Check.AddDeathEnemy(this);
        m_Agent.isStopped = true;
        m_IsDeath = true;
    }

    /// <summary>
    /// ���񂾓G�̐���0�Ȃ�false
    /// </summary>
    /// <returns></returns>
    public bool AnyDeadEnemies()
        => m_Check.DeathEnemyCount() != 0;

}
