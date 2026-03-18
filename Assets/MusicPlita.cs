using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlaySoundOnTrigger2D : MonoBehaviour
{
	public AudioClip soundClip;        // Аудиоклип
	[Range(0f, 1f)]
	public float volume = 1.0f;        // Громкость от 0 до 1

	private AudioSource audioSource;
	private bool hasPlayed = false;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;

		// Убедимся, что BoxCollider2D настроен как триггер
		BoxCollider2D box = GetComponent<BoxCollider2D>();
		box.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasPlayed && soundClip != null)
		{
			audioSource.PlayOneShot(soundClip, volume);
			hasPlayed = true;
		}
	}
}
