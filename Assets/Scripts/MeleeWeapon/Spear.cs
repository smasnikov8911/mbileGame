using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MeleeWeapon // опье - дальн€€ дистанци€, средний урон, средн€€ частота удра
{
    [SerializeField] private Animator animator;
    private bool attack = false;
    public override void Attack(Vector3 attackDirection)
    {
        if (attack)
        {
            return;
        }
        attack = true;
        SetAttackDirection(attackDirection);
        Debug.Log($"јтака копьем! ”рон: {damage}");
        if (animator != null)
        {
            Debug.Log("Spear" + attackDirection.x + attackDirection.y);
            animator.SetFloat("X", attackDirection.x);
            animator.SetFloat("Y", attackDirection.y);
            animator.SetBool("Attack", true);
        }
        else
        {
            Debug.LogWarning("Animator не назначен в Spear!");
        }
    }

    public override void ColliderAttack()
    {
        throw new System.NotImplementedException();
    }

    public void PerformAttack()
    {
        if (lastAttackDirection == Vector3.zero) return;

        Debug.Log("Spear.PerformAttack вызван из анимации");
        CreateAttackCollider(lastAttackDirection);
        animator.SetBool("Attack", false);
        attack = false;
    }

    public override void StopAttack()
    {
        throw new System.NotImplementedException();
    }
}
