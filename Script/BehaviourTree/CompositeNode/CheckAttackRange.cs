using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttackRange : CompositeNode
{
    [SerializeField, Min(0)]
    private int m_TrueNodeNum;//範囲内の時に選択される子ノードの要素番号

    [SerializeField, Min(0)]
    private int m_FalseNodeNum = 1;  //範囲外の時に選択される子ノードの要素番号

    [SerializeField]
    private float m_AttackRange = 5.0f;

    private bool m_InRange;

    protected override void OnStart()
    {
        m_InRange = false;
        float distance = context.enemy.CheckDistance();

        //攻撃範囲よりも近い場所にいる場合はSuccess
        if (m_AttackRange > distance)
        {
            m_InRange = true;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {

        if (m_InRange)
        {
            //範囲内なら範囲内用の子ノードを使用する
            return m_Children[m_TrueNodeNum].Update();
        }
        else
        {
            //範囲外なら範囲外用の子ノードを使用する
            return m_Children[m_FalseNodeNum].Update();
        }
    }
}