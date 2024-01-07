using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected CharacterAttack m_Attack;     //攻撃処理
    protected AnimationController m_Animation; //アニメーション
    protected EnemyColliderDisabler m_ColliderDisabler;

    protected NavMeshAgent m_Agent; //敵の移動をするためのAI

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

    protected bool m_IsDazzling; //眩しいかどうか
    private int m_MemoryMoveNumber = 0; //混乱時に移動のパラメータを記憶しておくための変数

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
        //攻撃用のチケットを作成するクラス
        new TicketedAttackManager();
    }

    /// <summary>
    /// プレイヤーを探す処理
    /// 見つけたらTrue
    /// 見つからない場合はFalse
    /// </summary>
    /// <returns></returns>
    public virtual bool SearchPlayer()
    {
        return m_IsPlayerFound;
    }

    /// <summary>
    /// プレイヤーが範囲内にいる場合は発見する
    /// </summary>
    /// <param name="player"></param>
    protected void PlayerInRange(Collider player)
    {
        Vector3 thisPos = transform.position + m_SearchRayOffset;
        Vector3 posDelta = player.transform.position + m_TargetRayOffset - thisPos;
        float targetAngle = Vector3.Angle(transform.forward, posDelta);

        if (targetAngle < m_SearchAngle)//視野の範囲内にいた場合
        {
            //壁などが邪魔している場合はスルーする
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
    /// 通常攻撃
    /// </summary>
    public abstract void Attack();
    /// <summary>
    /// 範囲攻撃
    /// </summary>
    public abstract void StrongAttack();

    public abstract void SpecialAttack();

    public abstract void Damage();

    /// <summary>
    /// 閃光玉が正面にあるかどうか
    /// </summary>
    /// <param name="flashPos"></param>
    public void CheckFlashInFront(Vector3 flashPos)
    {
        Vector3 toPos = flashPos - transform.position;
        float flashAngle = Vector3.Angle(transform.forward, toPos);

        if (flashAngle < m_SearchAngle)//視野の範囲内にいた場合
        {
            Dizzy();
        }
    }

    /// <summary>
    /// 混乱時の処理
    /// </summary>
    protected virtual void Dizzy()
    {
        Debugger.Log("混乱");
        m_Animation.Hit();
        m_Animation.SetMove(0);
        m_MemoryMoveNumber = m_Animation.GetMoveNumberInteger();
        m_IsDazzling = true;
        m_Agent.isStopped = true;
    }

    /// <summary>
    /// 混乱終了時の処理
    /// </summary>
    public virtual void DizzyEnd()
    {
        m_IsDazzling = false;
        m_Agent.isStopped = false;
        //記憶していた移動のパラメータにする
        m_Animation.SetMove(m_MemoryMoveNumber);
    }

    public virtual void Death()
    {
        m_Check.AddDeathEnemy(this);
        m_Agent.isStopped = true;
        m_IsDeath = true;
    }

    /// <summary>
    /// 死んだ敵の数が0ならfalse
    /// </summary>
    /// <returns></returns>
    public bool AnyDeadEnemies()
        => m_Check.DeathEnemyCount() != 0;

}
