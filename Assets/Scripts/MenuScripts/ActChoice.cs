using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;  // Для корутины

public class SceneLoader : MonoBehaviour
{
	public SceneAsset sceneToLoad;

	public void LoadScene()
	{
		if (sceneToLoad != null)
		{
			StartCoroutine(LoadSceneWithDelay());
		}
		else
		{
			Debug.LogError("Сцена не назначена!");
		}
	}

	private IEnumerator LoadSceneWithDelay()
	{
		yield return new WaitForSeconds(0.6f); // Задержка 1 секунда

		string scenePath = AssetDatabase.GetAssetPath(sceneToLoad);
		string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
		SceneManager.LoadScene(sceneName);
	}
}
