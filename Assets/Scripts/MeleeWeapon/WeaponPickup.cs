using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab; // Префаб оружия

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("SwordTriggger");
            PlayerWeaponHandler weaponHandler = other.GetComponent<PlayerWeaponHandler>();
            weaponHandler.EquipWeapon(weaponPrefab);
            Destroy(gameObject);
            
        }
    }
}
