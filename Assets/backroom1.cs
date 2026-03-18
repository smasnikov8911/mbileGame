using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class MusicZoneTrigger : MonoBehaviour
{
	[Header("Настройки музыки")]
	public AudioClip musicClip;
	[Range(0, 1)] public float volume = 1f;
	public bool loop = true;

	[Header("Настройки AudioSource")]
	[Range(0, 1)] public float spatialBlend = 0f;
	[Range(0.1f, 3f)] public float pitch = 1f;
	public bool playOnAwake = false;
	public bool bypassEffects = false;
	public bool bypassListenerEffects = false;
	public bool bypassReverbZones = false;
	[Range(0, 1.1f)] public float reverbZoneMix = 1f;
	public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
	[Range(0, 5)] public float dopplerLevel = 1f;
	[Range(0, 360)] public float spread = 0f;
	public float minDistance = 1f;
	public float maxDistance = 500f;
	public AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;
	public int priority = 128;

	[Header("Настройки перехода")]
	public float fadeInTime = 1f;
	public float fadeOutTime = 1f;

	private AudioSource audioSource;
	private bool playerInside = false;
	private Coroutine fadeCoroutine;

	private void Awake()
	{
		GetComponent<Collider2D>().isTrigger = true;
		InitializeAudioSource();
	}

	private void InitializeAudioSource()
	{
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = musicClip;
		audioSource.volume = 0f;
		audioSource.loop = loop;
		audioSource.playOnAwake = playOnAwake;

		audioSource.spatialBlend = spatialBlend;
		audioSource.pitch = pitch;
		audioSource.bypassEffects = bypassEffects;
		audioSource.bypassListenerEffects = bypassListenerEffects;
		audioSource.bypassReverbZones = bypassReverbZones;
		audioSource.reverbZoneMix = reverbZoneMix;
		audioSource.rolloffMode = rolloffMode;
		audioSource.dopplerLevel = dopplerLevel;
		audioSource.spread = spread;
		audioSource.minDistance = minDistance;
		audioSource.maxDistance = maxDistance;
		audioSource.velocityUpdateMode = velocityUpdateMode;
		audioSource.priority = priority;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerInside = true;
			StartFade(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerInside = false;
			StartFade(false);
		}
	}

	private void StartFade(bool fadeIn)
	{
		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}

		if (gameObject.activeInHierarchy)
		{
			fadeCoroutine = StartCoroutine(FadeMusic(fadeIn));
		}
		else if (!fadeIn)
		{
			audioSource.Stop();
		}
	}

	private IEnumerator FadeMusic(bool fadeIn)
	{
		float startVolume = audioSource.volume;
		float targetVolume = fadeIn ? volume : 0f;
		float duration = fadeIn ? fadeInTime : fadeOutTime;
		float elapsed = 0f;

		if (fadeIn && !audioSource.isPlaying)
		{
			audioSource.Play();
		}

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
			yield return null;
		}

		audioSource.volume = targetVolume;

		if (!fadeIn && !playerInside)
		{
			audioSource.Stop();
		}
	}

	private void OnDisable()
	{
		if (audioSource != null && audioSource.isPlaying)
		{
			if (fadeCoroutine != null)
			{
				StopCoroutine(fadeCoroutine);
				fadeCoroutine = null;
			}
			audioSource.Stop();
		}
	}

	private void OnDestroy()
	{
		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}
	}

	private void OnDrawGizmos()
	{
		if (GetComponent<Collider2D>() is BoxCollider2D boxCollider)
		{
			Gizmos.color = new Color(0, 1, 0, 0.3f);
			Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
		}
	}
}