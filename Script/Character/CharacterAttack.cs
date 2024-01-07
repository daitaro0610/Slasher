using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterAttack : MonoBehaviour
{
    private bool m_IsUpdateAttack = false; //Updateでアタックを呼び出し続ける際にTrueになる(ブレスなどの長期的な攻撃時)
    [SerializeField, Header("Update状態の時に何回に一回攻撃処理を行わせるのか")]
    private int m_AttackFrequencyInUpdates = 5;
    private int m_Counter; //FixedUpdateを何回行ったか確認用

    [SerializeField, Header("攻撃力")]
    private int[] m_Damages;
    private int m_CurrentDamageValue;

    [SerializeField, Header("攻撃用のコライダー")]
    private Collider[] m_AttackColliders;
    private Collider m_CurrentCollider;

    [SerializeField]
    private LayerMask m_DetectionLayer = -1;

    private void Awake()
    {
        m_IsUpdateAttack = false;
        m_Counter = 0;

        //初期状態ではは要素の1番目を指定する
        m_CurrentCollider = m_AttackColliders[0];
        m_CurrentDamageValue = m_Damages[0];
    }

    private void FixedUpdate()
    {
        //攻撃状態の時以外は処理を行わない
        if (!m_IsUpdateAttack) return;

        m_Counter++;
        //一定の回数待機したら攻撃処理を行う
        if (m_AttackFrequencyInUpdates < m_Counter)
        {
            m_Counter = 0;
            Attack();
        }
    }

    /// <summary>
    /// 持続的に攻撃処理を行う際に呼ぶ
    /// </summary>
    /// <param name="time">攻撃処理の時間</param>
    /// <returns></returns>
    public CharacterAttack UpdateAttackEnabled(bool isbool)
    {
        m_IsUpdateAttack = isbool;
        m_Counter = 0;
        return this;
    }

    /// <summary>
    /// 攻撃力を変更する
    /// </summary>
    /// <param name="index">m_Damagesの要素番号</param>
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
            //すり抜けるコライダーは判定しない
            if (collider.isTrigger) continue;

            IDamageble damageble = collider.GetComponent<IDamageble>();

            //同じオブジェクトの場合はもうダメージ処理を行わない
            if (damageTargetList.Contains(collider.transform.root.gameObject)) continue;

            //自身のコライダーは無視する
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
        //攻撃の処理を呼びだしたタイミングでm_AttackColliderに衝突しているほかのコライダーを取得する
        //Boxコライダーの場合
        if (m_CurrentCollider is BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
            Vector3 halfExtents = boxCollider.size * 0.5f;
            Quaternion rotation = boxCollider.transform.rotation;
            return Physics.OverlapBox(center, halfExtents, rotation, m_DetectionLayer);
        }
        //カプセルコライダーの場合
        else if (m_CurrentCollider is CapsuleCollider capsuleCollider)
        {
            Vector3 point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + 0.5f * capsuleCollider.height * Vector3.up);
            Vector3 point2 = capsuleCollider.transform.TransformPoint(capsuleCollider.center - 0.5f * capsuleCollider.height * Vector3.up);
            float radius = capsuleCollider.radius;
            return Physics.OverlapCapsule(point1, point2, radius, m_DetectionLayer);
        }
        //スフィアコライダーの場合
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
        //スフィアコライダーの場合
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
                    //X軸に向いている
                    direction = Vector3.right;
                    break;
                case 1:
                    //Y軸に向いている
                    direction = Vector3.up;
                    break;
                case 2:
                    //Z軸に向いている
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
