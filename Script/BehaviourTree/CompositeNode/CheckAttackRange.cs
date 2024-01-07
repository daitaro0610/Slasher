using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttackRange : CompositeNode
{
    [SerializeField, Min(0)]
    private int m_TrueNodeNum;//�͈͓��̎��ɑI�������q�m�[�h�̗v�f�ԍ�

    [SerializeField, Min(0)]
    private int m_FalseNodeNum = 1;  //�͈͊O�̎��ɑI�������q�m�[�h�̗v�f�ԍ�

    [SerializeField]
    private float m_AttackRange = 5.0f;

    private bool m_InRange;

    protected override void OnStart()
    {
        m_InRange = false;
        float distance = context.enemy.CheckDistance();

        //�U���͈͂����߂��ꏊ�ɂ���ꍇ��Success
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
            //�͈͓��Ȃ�͈͓��p�̎q�m�[�h���g�p����
            return m_Children[m_TrueNodeNum].Update();
        }
        else
        {
            //�͈͊O�Ȃ�͈͊O�p�̎q�m�[�h���g�p����
            return m_Children[m_FalseNodeNum].Update();
        }
    }
}