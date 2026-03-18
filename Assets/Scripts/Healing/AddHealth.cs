using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	public enum HealthAmount
	{
		HalfHeart,
		OneHeart,
		TwoHearts
	}

	public HealthAmount healthAmount; // Количество здоровья, которое восстанавливается
    public ParticleSystem particleSystem; // Ссылка на систему частиц

    void Start()
    {
        // Убедитесь, что партиклы выключены изначально
        if (particleSystem != null)
        {
            particleSystem.Stop(); // Останавливаем партиклы
            particleSystem.Clear(); // Очищаем партиклы
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
	{
        // Проверяем, если объект, с которым мы столкнулись, имеет нужный тег
        if (other.CompareTag("Player")) // Замените "Player" на нужный вам тег
        {
            ActivateParticles();
        }
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
		if (playerHealth != null)
		{
			switch (healthAmount)
			{
				case HealthAmount.HalfHeart:
					playerHealth.RestoreHealth(HeartStatus.Half);
					break;
				case HealthAmount.OneHeart:
					playerHealth.RestoreHealth(HeartStatus.Full);
					break;
				case HealthAmount.TwoHearts:
					playerHealth.RestoreHealth(HeartStatus.Full);
					playerHealth.RestoreHealth(HeartStatus.Full); // Восстанавливаем два сердца
					break;
			}

			Destroy(gameObject); // Удаляем объект после использования
		}
	}
    void ActivateParticles()
    {
        if (particleSystem != null)
        {
            particleSystem.Play(); // Запускаем партиклы
        }
    }
}