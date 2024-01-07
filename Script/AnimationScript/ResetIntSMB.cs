using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIntSMB : StateMachineBehaviour
{
    [SerializeField]
    private string m_ParameterName;
    [SerializeField]
    private int m_ResetValue;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(m_ParameterName,m_ResetValue);
    }
}
