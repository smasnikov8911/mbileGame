using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HeartStatus
{
	Full,
	Half,
	Empty
}

[System.Serializable]
public class Heart
{
	public HeartStatus status;
}

public class PlayerHealth : MonoBehaviour
{
	public List<Heart> hearts; // Список сердец
	public Image[] heartImages; // Массив изображений сердец в UI

	public Sprite fullHeartSprite;
	public Sprite halfHeartSprite;
	public Sprite emptyHeartSprite;

	private SpriteRenderer spriteRenderer;
	public GameObject gameOverScreen;

	// Звуковые эффекты
	public AudioClip[] halfHeartSounds; // Звуки при потере половины сердца
	public AudioClip[] restoreSounds;   // Звуки при восстановлении сердца
	public AudioClip deathSound;        // Звук смерти при окончании всех сердец
	private AudioSource audioSource;    // Основной источник звука
	private bool isFlashing = false;

	// Громкость для разных типов звуков
	[Range(0f, 1f)] public float halfHeartVolume = 0.8f;  // Громкость для звуков потери половины сердца
	[Range(0f, 1f)] public float restoreVolume = 0.8f;    // Громкость для звуков восстановления
	[Range(0f, 1f)] public float deathVolume = 1f;         // Громкость для звука смерти

	private void Start()
	{
		Time.timeScale = 1; // Возвращаем нормальную скорость игры

		spriteRenderer = GetComponent<SpriteRenderer>();

		// Настройка AudioSource
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.spatialBlend = 0; // 2D-звук
		audioSource.playOnAwake = false;

		if (PlayerPrefs.HasKey("GameRestarted"))
		{
			ResetHealth();
			PlayerPrefs.DeleteKey("GameRestarted");
		}
		else if (PlayerPrefs.HasKey("PlayerHealth"))
		{
			int savedHealth = PlayerPrefs.GetInt("PlayerHealth");
			LoadHealth(savedHealth);
		}
		else
		{
			InitializeHealth();
		}

		UpdateHealthBar();
		gameOverScreen.SetActive(false);
	}

	public void InitializeHealth(int totalHearts = 3)
	{
		hearts = new List<Heart>();
		for (int i = 0; i < totalHearts; i++)
		{
			hearts.Add(new Heart { status = HeartStatus.Full });
		}
	}

	public void TakeDamage()
	{
		if (isFlashing) return;

		for (int i = hearts.Count - 1; i >= 0; i--)
		{
			if (hearts[i].status == HeartStatus.Full)
			{
				hearts[i].status = HeartStatus.Half;
				UpdateHealthBar();
				StartCoroutine(FlashRed());
				SaveHealth();

				// Проигрываем звук потери половины сердца с заданной громкостью
				PlayRandomSound(halfHeartSounds, halfHeartVolume);
				return;
			}
			else if (hearts[i].status == HeartStatus.Half)
			{
				hearts[i].status = HeartStatus.Empty;
				UpdateHealthBar();
				StartCoroutine(FlashRed());
				CheckGameOver();
				SaveHealth();
				return;
			}
		}
	}

	private void CheckGameOver()
	{
		foreach (var heart in hearts)
		{
			if (heart.status != HeartStatus.Empty)
			{
				return; // Если есть хоть одно не пустое сердце, игра продолжается
			}
		}

		// Если все сердца пустые, показываем экран Game Over
		gameOverScreen.SetActive(true);
		Time.timeScale = 0; // Останавливаем игру

		// Проигрываем звук смерти с заданной громкостью
		if (deathSound != null && audioSource != null)
		{
			audioSource.PlayOneShot(deathSound, deathVolume);
		}

		// Удаляем сохранённое здоровье
		PlayerPrefs.DeleteKey("PlayerHealth");
		PlayerPrefs.SetInt("GameRestarted", 1);
		PlayerPrefs.Save();
	}

	private IEnumerator FlashRed()
	{
		// Если уже мигаем, выходим
		if (isFlashing) yield break;

		isFlashing = true;
		Color originalColor = spriteRenderer.color;
		Color darkRed = new Color(1f, 0f, 0f, 0.5f);

		spriteRenderer.color = darkRed;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = originalColor;
		isFlashing = false;
	}

	public void RestoreHealth(HeartStatus heartStatus)
	{
		if (heartStatus == HeartStatus.Full)
		{
			for (int i = 0; i < hearts.Count; i++)
			{
				if (hearts[i].status == HeartStatus.Empty)
				{
					hearts[i].status = HeartStatus.Full;
					UpdateHealthBar();
					PlayRandomSound(restoreSounds, restoreVolume);
					return;
				}
				else if (hearts[i].status == HeartStatus.Half)
				{
					hearts[i].status = HeartStatus.Full;
					UpdateHealthBar();
					PlayRandomSound(restoreSounds, restoreVolume);

					for (int j = i + 1; j < hearts.Count; j++)
					{
						if (hearts[j].status == HeartStatus.Empty)
						{
							hearts[j].status = HeartStatus.Half;
							UpdateHealthBar();
							return;
						}
					}
					return;
				}
			}
		}
		if (heartStatus == HeartStatus.Half)
		{
			for (int i = 0; i < hearts.Count; i++)
			{
				if (hearts[i].status == HeartStatus.Empty)
				{
					hearts[i].status = HeartStatus.Half;
					UpdateHealthBar();
					PlayRandomSound(restoreSounds, restoreVolume);
					return;
				}
				else if (hearts[i].status == HeartStatus.Half)
				{
					hearts[i].status = HeartStatus.Full;
					UpdateHealthBar();
					PlayRandomSound(restoreSounds, restoreVolume);
					return;
				}
			}
		}
	}

	// Метод для воспроизведения случайного звука с наложением и с учетом громкости
	private void PlayRandomSound(AudioClip[] clips, float volume)
	{
		if (clips == null || clips.Length == 0 || audioSource == null)
			return;

		AudioClip randomClip = clips[Random.Range(0, clips.Length)];
		audioSource.PlayOneShot(randomClip, volume); // Звуки накладываются с указанной громкостью
	}

	public void UpdateHealthBar()
	{
		if (heartImages == null || hearts == null)
		{
			Debug.LogWarning("heartImages или hearts не инициализированы!");
			return;
		}

		for (int i = 0; i < heartImages.Length; i++)
		{
			if (heartImages[i] == null)
			{
				Debug.LogWarning($"heartImages[{i}] == null");
				continue;
			}

			if (i < hearts.Count)
			{
				switch (hearts[i].status)
				{
					case HeartStatus.Full:
						heartImages[i].sprite = fullHeartSprite;
						break;
					case HeartStatus.Half:
						heartImages[i].sprite = halfHeartSprite;
						break;
					case HeartStatus.Empty:
						heartImages[i].sprite = emptyHeartSprite;
						break;
				}
			}
			else
			{
				heartImages[i].enabled = false;
			}
		}
	}

	private void SaveHealth()
	{
		int healthValue = 0;

		foreach (var heart in hearts)
		{
			if (heart.status == HeartStatus.Full)
				healthValue += 2;
			else if (heart.status == HeartStatus.Half)
				healthValue += 1;
		}

		PlayerPrefs.SetInt("PlayerHealth", healthValue);
		PlayerPrefs.Save();
	}

	private void LoadHealth(int healthValue)
	{
		hearts = new List<Heart>();

		for (int i = 0; i < 3; i++)
		{
			if (healthValue >= 2)
			{
				hearts.Add(new Heart { status = HeartStatus.Full });
				healthValue -= 2;
			}
			else if (healthValue == 1)
			{
				hearts.Add(new Heart { status = HeartStatus.Half });
				healthValue -= 1;
			}
			else
			{
				hearts.Add(new Heart { status = HeartStatus.Empty });
			}
		}
	}

	private void ResetHealth()
	{
		InitializeHealth();
		SaveHealth();
	}
}
