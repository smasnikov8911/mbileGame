using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MeleeWeapon //Меч - средняя дистанция, средний урон, средняя частота удра
{
    [SerializeField] private Animator animator;
    private bool attack = false;
    public void SetAnimator(Animator externalAnimator)
    {
        animator = externalAnimator;
    }
    public override void Attack(Vector3 attackDirection)
    {
        if (attack)
        {
            return;
        }
        attack = true;
        SetAttackDirection(attackDirection);
        Debug.Log($"Атака мечом! Урон: {damage}");
        if (animator != null)
        {
            Debug.Log("Swor" + attackDirection.x + attackDirection.y);
            animator.SetFloat("X", attackDirection.x);
            animator.SetFloat("Y", attackDirection.y);
            animator.SetBool("Attack", true);
        }
        else
        {
            Debug.LogWarning("Animator не назначен в Sword!");
        }
    }

    public override void ColliderAttack()
    {
        Debug.Log("Sword.PerformAttack вызван из анимации");
        CreateAttackCollider(lastAttackDirection);
    }    
    public override void StopAttack()
    {
        if (lastAttackDirection == Vector3.zero) return;

        Debug.Log("Sword.PerformAttack вызван из анимации");
        animator.SetBool("Attack", false);
        attack = false;
    }
}
