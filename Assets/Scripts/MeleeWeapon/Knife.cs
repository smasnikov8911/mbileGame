using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MeleeWeapon //Кинжал - ближняя дистанция, маленький урон, большая частота удра
{
    public override void Attack(Vector3 attackDirection)
    {
        SetAttackDirection(attackDirection);
        Debug.Log($"Атака мечом! Урон: {damage}");
        //Todo мб будущая логика для оружия, хотя предлагю вызывать метод при таймкеях
        //Ниже вызывать анимацию для оружия
    }

    public override void ColliderAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void StopAttack()
    {
        throw new System.NotImplementedException();
    }
}
