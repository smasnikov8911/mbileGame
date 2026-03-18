using UnityEngine;

[RequireComponent(typeof(Collider))] // Требуем наличие коллайдера
[RequireComponent(typeof(AudioSource))] // Требуем наличие AudioSource
public class PlayMusicOnTrigger : MonoBehaviour
{
	[Header("Настройки музыки")]
	[Tooltip("Фоновая музыка")]
	public AudioClip backgroundMusic;

	[Range(0f, 1f)]
	public float volume = 0.5f;

	public bool loop = true;
	public bool playOnAwake = false;

	private AudioSource audioSource;
	private bool wasPlayed = false;

	void Start()
	{
		// Настраиваем AudioSource
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = backgroundMusic;
		audioSource.volume = volume;
		audioSource.loop = loop;
		audioSource.playOnAwake = playOnAwake;

		// Делаем коллайдер триггером
		GetComponent<Collider>().isTrigger = true;
	}

	void OnTriggerEnter(Collider other)
	{
		// Проверяем, что вошел игрок (по тегу)
		if (other.CompareTag("Player") && !wasPlayed)
		{
			// Воспроизводим музыку
			audioSource.Play();
			wasPlayed = true;

			// Можно добавить дополнительные эффекты
			Debug.Log("Фоновая музыка начата");
		}
	}

	// Опционально: остановка музыки при выходе из триггера
	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			// audioSource.Stop(); // Раскомментируйте, если нужно останавливать музыку
		}
	}
}