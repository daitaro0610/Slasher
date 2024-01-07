using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController
{
    private Animator m_Animator;

    public AnimationController(GameObject gameObject)
    {
        m_Animator = gameObject.GetComponent<Animator>();
    }

    public void SetIdle(float value) => m_Animator.SetFloat(AnimationHash.Idle, value);
    public void SetMove(float value) => m_Animator.SetFloat(AnimationHash.Move, value);
    public void SetMove(int value) => m_Animator.SetInteger(AnimationHash.Move, value);
    public int GetMoveNumberInteger() => m_Animator.GetInteger(AnimationHash.Move);
    public void IsDash(bool isbool) => m_Animator.SetBool(AnimationHash.IsDash, isbool);
    public void DrawingSword() => m_Animator.SetTrigger(AnimationHash.DrawingSword);
    public void ReturnSword() => m_Animator.SetTrigger(AnimationHash.ReturnSword);
    public void Crouch(bool isbool) => m_Animator.SetBool(AnimationHash.IsCrouch, isbool);
    public void Guard() => m_Animator.SetTrigger(AnimationHash.Guard);
    public void SetGuard(bool isbool) => m_Animator.SetBool(AnimationHash.IsGuard, isbool);
    public void Attack() => m_Animator.SetTrigger(AnimationHash.Attack);
    public void SubAttack() => m_Animator.SetTrigger(AnimationHash.SubAttack);
    public void Attack(int num)
    {
        switch (num)
        {
            case 0:
                m_Animator.SetTrigger(AnimationHash.Attack);
                break;
            case 1:
                m_Animator.SetTrigger(AnimationHash.StrongAttack);
                break;
            case 2:
                m_Animator.SetTrigger(AnimationHash.Flame);
                break;
            case 3:
                m_Animator.SetTrigger(AnimationHash.FlyFlame);
                break;
        }
    }
    public void Flame() => m_Animator.SetTrigger(AnimationHash.Flame);
    public void Fly() => m_Animator.SetTrigger(AnimationHash.Fly);
    public void Land() => m_Animator.SetTrigger(AnimationHash.Land);
    public void Jump() => m_Animator.SetTrigger(AnimationHash.Jump);
    public void Avoidance() => m_Animator.SetTrigger(AnimationHash.Avoidance);
    public void AvoidanceVelocity(float value) => m_Animator.SetFloat(AnimationHash.AvoidanceVelocity, value);
    public void Counter() => m_Animator.SetTrigger(AnimationHash.Counter);
    public void Counter(bool isbool) => m_Animator.SetBool(AnimationHash.IsCounter, isbool);
    public void SetFall(bool isbool) => m_Animator.SetBool(AnimationHash.IsFall, isbool);
    public void Damage() => m_Animator.SetTrigger(AnimationHash.Damage);
    public void SetDamageNumber(int num) => m_Animator.SetInteger(AnimationHash.DamageNumber, num);
    public void Death() => m_Animator.SetTrigger(AnimationHash.Death);
    public void Roar() => m_Animator.SetTrigger(AnimationHash.Roar);
    public void AngryRoar() => m_Animator.SetTrigger(AnimationHash.AngryRoar);
    public void Hit() => m_Animator.SetTrigger(AnimationHash.Hit);
    public void Clear() => m_Animator.SetTrigger(AnimationHash.Clear);

}
