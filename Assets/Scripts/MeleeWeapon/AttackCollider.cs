using UnityEngine;

public class AttackCollider : MonoBehaviour
{
	[Header("Настройки урона")]
	private int damage;

	[Header("Настройки звуков")]
	public AudioClip hitSound; // Звук попадания
	public AudioClip missSound; // Звук промаха
	[Range(0, 1)] public float volume = 1f;

	private bool hasHit = false;

	public void SetDamage(int dmg)
	{
		damage = dmg;
		Debug.Log($"Урон установлен: {damage}");
	}

	void OnEnable()
	{
		hasHit = false; // Сброс при каждом создании коллайдера
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
			{
				enemyHealth.TakeDamage(damage);
				PlaySound(hitSound);
				hasHit = true;
			}
		}
	}

	void OnDisable()
	{
		// Проигрываем звук промаха если не было попадания
		if (!hasHit && missSound != null)
		{
			AudioSource.PlayClipAtPoint(missSound, transform.position, volume);
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if (clip != null)
		{
			// Создаем временный AudioSource для звука
			AudioSource.PlayClipAtPoint(clip, transform.position, volume);
		}
	}
}