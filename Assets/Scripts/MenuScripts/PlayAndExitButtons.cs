using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // Для корутин

public class MainMenuController : MonoBehaviour
{
	private const string WEAPON_KEY = "CurrentWeapon";

	public void PlayGame()
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		PlayerPrefs.DeleteKey(WEAPON_KEY);
		PlayerPrefs.Save();

		StartCoroutine(LoadSceneWithDelay(currentSceneIndex, 0.6f));
	}

	public void PlayMenu()
	{
		StartCoroutine(LoadSceneWithDelay(0, 0.6f));
	}

	public void ExitGame()
	{
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private IEnumerator LoadSceneWithDelay(int sceneIndex, float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(sceneIndex);
	}
}
