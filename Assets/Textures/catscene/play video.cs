using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor; // Для использования SceneAsset

public class SceneLoaderButton : MonoBehaviour
{
	[Header("Видео")]
	public VideoPlayer videoPlayer; // Видео плеер для катсцены
	public GameObject videoScreen; // Экран для видео

	[Header("Эффекты")]
	public GameObject fadeScreen; // UI Image для затемнения
	public string sceneToLoad; // Имя сцены для загрузки
	public LoadingScreen loadingScreen;
	public Button button;

	[Header("Настройки сцен")]
	public int requiredLevelToUnlock = 1;
	private bool isUnlocked = false;

	void Start()
	{
		if (videoScreen != null) videoScreen.SetActive(false);
		if (fadeScreen != null) fadeScreen.SetActive(false);
		CheckLevelProgress();

		if (videoPlayer != null)
			videoPlayer.loopPointReached += OnVideoEnd;
	}

	private void CheckLevelProgress()
	{
		// Получаем текущий прогресс
		int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted", 0);

		// Разблокируем если игрок прошел нужный уровень
		isUnlocked = levelsCompleted >= requiredLevelToUnlock - 1;

		if (button != null)
		{
			button.interactable = isUnlocked;

			// Визуальная индикация (необязательно)
			if (!isUnlocked)
			{
				button.image.color = Color.gray;
				button.GetComponentInChildren<Text>().text = $"Требуется уровень {requiredLevelToUnlock}";
			}
		}
	}


	public void OnButtonClick()
	{
		if (!isUnlocked) return;

		if (videoPlayer != null && videoScreen != null)
		{
			Debug.Log("Запуск катсцены!");
			videoScreen.SetActive(true);
			videoPlayer.Play();
		}
		else
		{
			loadingScreen.Loading(sceneToLoad);
		}
	}

	private void OnVideoEnd(VideoPlayer vp)
	{
		Debug.Log("Катсцена закончилась, загружаем сцену!");
		StartCoroutine(FadeAndLoadScene()); // Затемняем экран и загружаем сцену
	}

	IEnumerator FadeAndLoadScene()
	{
		if (fadeScreen != null)
		{
			fadeScreen.SetActive(true);
			Image fadeImage = fadeScreen.GetComponent<Image>();
			Color color = fadeImage.color;

			// Плавное затемнение
			float duration = 0.5f;
			for (float t = 0; t < duration; t += Time.deltaTime)
			{
				color.a = Mathf.Lerp(0, 1, t / duration);
				fadeImage.color = color;
				yield return null;
			}
		}

		// Загружаем новую сцену
		loadingScreen.Loading(sceneToLoad);
	}

}