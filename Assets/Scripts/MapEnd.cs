#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TriggerSceneLoader : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset sceneToLoad; // Сцена, которую нужно загрузить
#else
    public string sceneToLoad; // Имя сцены, которую нужно загрузить
#endif
    public int levelNumber = 1;
    public Image darkOverlay; // Ссылка на Image для затемнения
    public LoadingScreen loadingScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Сохраняем прогресс
            int currentProgress = PlayerPrefs.GetInt("LevelsCompleted", 0);
            if (levelNumber > currentProgress)
            {
                PlayerPrefs.SetInt("LevelsCompleted", levelNumber);
                PlayerPrefs.Save();
            }

            // Загрузка следующей сцены
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Затемняем экран
        float fadeDuration = 1f; // Длительность затемнения
        float elapsedTime = 0f;

        Color overlayColor = darkOverlay.color;
        overlayColor.a = 0; // Начинаем с прозрачного

        // Увеличиваем альфа-канал до 1 (полностью черный)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            overlayColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            darkOverlay.color = overlayColor;
            yield return null;
        }

#if UNITY_EDITOR
        string sceneName = sceneToLoad.name;
#else
        string sceneName = sceneToLoad;
#endif

        // Загружаем новую сцену
        loadingScreen.Loading(sceneName);
    }
}