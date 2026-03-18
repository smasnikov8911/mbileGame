using UnityEngine;

public class DamageDealer : MonoBehaviour
{
	public int damageAmount = 1; // Количество урона
	private PlayerHealth playerHealth;

	private void Start()
	{
		playerHealth = FindObjectOfType<PlayerHealth>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			for (int i = 0; i < damageAmount; i++)
			{
				playerHealth.TakeDamage();
				playerHealth.UpdateHealthBar();
			}

			Destroy(gameObject);
		}


	}
}