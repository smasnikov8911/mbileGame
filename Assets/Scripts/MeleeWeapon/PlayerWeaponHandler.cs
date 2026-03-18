using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponHandler : MonoBehaviour
{
	[SerializeField] private Transform weaponHolder;
	[SerializeField] private Joystick joystick;
	[SerializeField] private MeleeWeapon equippedWeapon;

	[SerializeField] private GameObject currentWeapon;
	[SerializeField] private Image weaponIcon;

	private PlayerController playerController;
	private const string WEAPON_KEY = "CurrentWeapon";

	[SerializeField] private AudioClip pickupSound;
	[SerializeField][Range(0f, 1f)] private float soundVolume = 1f;
	private AudioSource audioSource;

	private void Start()
	{
		playerController = GetComponentInParent<PlayerController>();
		audioSource = GetComponent<AudioSource>();

		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		LoadWeapon(); // загрузка без воспроизведения звука
	}

	// Добавлен параметр playSound
	public void EquipWeapon(GameObject weaponPrefab, bool playSound = true)
	{
		if (currentWeapon != null)
		{
			Destroy(currentWeapon);
		}

		currentWeapon = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.Euler(0, 0, 45), weaponHolder);
		equippedWeapon = currentWeapon.GetComponent<MeleeWeapon>();

		PlayerPrefs.SetString(WEAPON_KEY, weaponPrefab.name);
		PlayerPrefs.Save();

		UpdateWeaponIcon();
		Debug.Log("EQUPPED");

		if (playSound)
		{
			PlayPickupSound();
		}

		Animator playerAnimator = GameObject.Find("PlayerVisual")?.GetComponent<Animator>();

		if (playerAnimator != null)
		{
			Sword sword = equippedWeapon as Sword;
			if (sword != null)
			{
				sword.SetAnimator(playerAnimator);
			}
		}
		else
		{
			Debug.LogWarning("PlayerVisual или его Animator не найден!");
		}
	}

	private void PlayPickupSound()
	{
		if (pickupSound != null)
		{
			audioSource.PlayOneShot(pickupSound, soundVolume);
		}
	}

	private void LoadWeapon()
	{
		if (PlayerPrefs.HasKey(WEAPON_KEY))
		{
			string weaponName = PlayerPrefs.GetString(WEAPON_KEY);
			GameObject weaponPrefab = Resources.Load<GameObject>("GameObjects/Weapons/" + weaponName);

			if (weaponPrefab != null)
			{
				EquipWeapon(weaponPrefab, false); // не воспроизводим звук при загрузке
			}
			else
			{
				Debug.LogError("Weapon prefab not found: " + weaponName);
			}
		}
	}

	private void UpdateWeaponIcon()
	{
		if (weaponIcon == null)
		{
			Debug.LogWarning("Weapon icon UI is not assigned!");
			return;
		}

		if (equippedWeapon != null && equippedWeapon.icon != null)
		{
			weaponIcon.sprite = equippedWeapon.icon;
			weaponIcon.enabled = true;
		}
		else
		{
			weaponIcon.enabled = false;
		}
	}

	public void PlayerAttack()
	{
		if (equippedWeapon == null)
		{
			Debug.LogWarning("No weapon equipped!");
			return;
		}

		Vector2 attackDirection = playerController.GetLastMoveDirection();
		equippedWeapon.Attack(attackDirection);
	}	
	
	public void PlayerAttackCollider()
	{
		if (equippedWeapon == null)
		{
			Debug.LogWarning("No weapon equipped!");
			return;
		}
		equippedWeapon.ColliderAttack();
	}	
	public void PlayerAttackColliderAnim()
	{
		if (equippedWeapon == null)
		{
			Debug.LogWarning("No weapon equipped!");
			return;
		}
		equippedWeapon.StopAttack();
	}
}
