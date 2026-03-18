using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeleeWeapon
{
    int Damage { get; }
    void Attack(Vector3 attackDirection);
}
