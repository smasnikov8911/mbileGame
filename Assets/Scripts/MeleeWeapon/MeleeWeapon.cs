using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeWeapon : MonoBehaviour, IMeleeWeapon
{
	[SerializeField] protected GameObject attackColliderPrefab;
	[SerializeField] protected int damage;
	[SerializeField] public Sprite icon;

	protected Vector3 lastAttackDirection;

	public int Damage => damage;

	public void SetAttackDirection(Vector3 attackDirection)
	{
		lastAttackDirection = attackDirection.normalized;
	}

	protected void CreateAttackCollider(Vector3 attackDirection)
	{
		if (attackColliderPrefab == null)
		{
			Debug.LogWarning("AttackCollider prefab reference is missing!");
			return;
		}

		// ������������ ���� �������� ��� ����������
		float angle = CalculateAttackAngle(attackDirection);
		Debug.Log("Calculated angle: " + angle + " for direction: " + attackDirection);
		Quaternion rotation = Quaternion.Euler(0, 0, angle);

		// ������������� � ������������ ���������
		attackColliderPrefab.transform.localRotation = Quaternion.identity;
		attackColliderPrefab.transform.localRotation = Quaternion.Euler(0, 0, angle);
		Debug.Log("Actual rotation: " + attackColliderPrefab.transform.localEulerAngles.z);
		// ����������� ����
		AttackCollider attackCollider = attackColliderPrefab.GetComponent<AttackCollider>();
		if (attackCollider == null) return;

		attackCollider.SetDamage(damage);
		attackColliderPrefab.SetActive(true);
		StartCoroutine(DisableAfterDelay(attackColliderPrefab, 0.05f));
	}

	private float CalculateAttackAngle(Vector3 direction)
	{
		// ����������� ������ �����������
		direction.Normalize();

		// ���������� �������� ����������� � ������� ~0.7 (�������� 45� �� ����)
		if (direction.y < -0.9f) return 315f;    // ����� ���� (315�)
		if (direction.y > 0.9f) return 135f;      // ����� ����� (135�)
		if (direction.x < -0.9f) return 225f;     // ����� ����� (225�)
		if (direction.x > 0.9f) return 45f;

		// ��� ������������ ����������� �������� ��������� ����
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		if (angle >= -45f && angle < 45f) return 45f;      // ����� (45�)
		if (angle >= 45f && angle < 135f) return 135f;      // ����� (135�)
		if (angle >= 135f || angle < -135f) return 225f;    // ����� (225�)
		return 315f;                                        // ���� (315�)
	}

	private IEnumerator DisableAfterDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		obj.SetActive(false);
	}

	public abstract void Attack(Vector3 attackDirection);
	public abstract void StopAttack();
	public abstract void ColliderAttack();

}