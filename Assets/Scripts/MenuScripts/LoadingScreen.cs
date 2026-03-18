using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider scale;
    public float artificialDelay = 0.5f;

    public void Loading(string sceneToLoad)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsync(sceneToLoad));
    }
    IEnumerator LoadAsync(string scene)
    {
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(scene);
        loadAsync.allowSceneActivation = false;

        float progress = 0f;
        float timer = 0f;

        while (!loadAsync.isDone)
        {
            // Реальный прогресс загрузки (0-0.9)
            float realProgress = loadAsync.progress;

            // Искусственное "доливание" прогресса до 1
            progress = Mathf.Clamp01(realProgress / 0.9f);

            // Обновляем Slider
            scale.value = progress;

            // Когда загрузка почти завершена
            if (realProgress >= 0.9f)
            {
                // Добавляем небольшую задержку
                timer += Time.deltaTime;
                if (timer >= artificialDelay)
                {
                    loadAsync.allowSceneActivation = true;
                }
            }

            yield return null;
        }

    }
}