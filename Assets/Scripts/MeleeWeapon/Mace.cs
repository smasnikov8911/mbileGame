using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MeleeWeapon //Ѕулава - дальн€€ дистанци€, большой урон, маленька€ частота удра
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
        Debug.Log($"јтака булавой! ”рон: {damage}");
        if (animator != null)
        {
            Debug.Log("Mace" + attackDirection.x + attackDirection.y);
            animator.SetFloat("X", attackDirection.x);
            animator.SetFloat("Y", attackDirection.y);
            animator.SetBool("Attack", true);
        }
        else
        {
            Debug.LogWarning("Animator не назначен в Mace!");
        }
    }

    public override void ColliderAttack()
    {
        throw new System.NotImplementedException();
    }

    public void PerformAttack()
    {
        if (lastAttackDirection == Vector3.zero) return;

        Debug.Log("Mace.PerformAttack вызван из анимации");
        CreateAttackCollider(lastAttackDirection);
        animator.SetBool("Attack", false);
        attack = false;
    }

    public override void StopAttack()
    {
        throw new System.NotImplementedException();
    }
}
