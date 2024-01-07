using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTriggerSMB : StateMachineBehaviour
{
    [SerializeField]
    private string m_TriggerName;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(m_TriggerName);
    }
}
