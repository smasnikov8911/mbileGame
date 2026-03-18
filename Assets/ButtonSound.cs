using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlaySoundOnClick : MonoBehaviour
{
	[Header("Audio Settings")]
	public AudioClip clickSound; // Звук нажатия
	[Range(0f, 1f)]
	public float volume = 1.0f; // Громкость звука

	[Header("Scene Settings")]
	public string nextSceneName; // Имя следующей сцены
	public float sceneChangeDelay = 0.5f; // Задержка перед переключением сцены

	private AudioSource audioSource;

	private void Start()
	{
		// Добавляем компонент AudioSource, если его нет
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		audioSource.playOnAwake = false; // Звук не должен проигрываться автоматически
	}

	private void OnMouseDown()
	{
		// Проигрываем звук
		if (clickSound != null && audioSource != null)
		{
			audioSource.PlayOneShot(clickSound, volume);
		}

		// Запускаем корутину для смены сцены с задержкой
		if (!string.IsNullOrEmpty(nextSceneName))
		{
			StartCoroutine(LoadSceneWithDelay(sceneChangeDelay));
		}
	}

	private IEnumerator LoadSceneWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(nextSceneName);
	}
}
