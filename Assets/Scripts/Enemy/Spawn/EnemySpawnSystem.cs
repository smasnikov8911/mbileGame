using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawnSystem : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform point;
        [Dropdown("enemyNames")]
        public string enemyPrefabName;
    }

    private List<string> enemyNames = new List<string>();

    [Header("Настройки спавна")]
    [Tooltip("Папка в Resources где лежат префабы врагов")]
    private string enemiesFolder = "GameObjects/Enemy";

    [Space]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    public float spawnDelay = 0.5f;

    private Dictionary<string, GameObject> enemyPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Debug.Log($"Полный путь к Resources: {Application.dataPath}/Resources/{enemiesFolder}");
        LoadEnemyPrefabs();
        UpdateEnemyNamesList();
    }

    private void Start()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (enemyPrefabs.ContainsKey(spawnPoint.enemyPrefabName))
            {
                Instantiate(
                    enemyPrefabs[spawnPoint.enemyPrefabName],
                    spawnPoint.point.position,
                    spawnPoint.point.rotation
                );
            }
            else
            {
                Debug.LogWarning($"Префаб врага {spawnPoint.enemyPrefabName} не найден!");
            }

        }
    }

    private void LoadEnemyPrefabs()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>(enemiesFolder);
        Debug.Log("LOADER PREFAPBS" + loadedPrefabs);
        foreach (var prefab in loadedPrefabs)
        {
            enemyPrefabs.Add(prefab.name, prefab);
        }
    }

    private void UpdateEnemyNamesList()
    {
        enemyNames = enemyPrefabs.Keys.ToList();
    }

    private IEnumerator SpawnEnemiesWithDelay()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (enemyPrefabs.ContainsKey(spawnPoint.enemyPrefabName))
            {
                Instantiate(
                    enemyPrefabs[spawnPoint.enemyPrefabName],
                    spawnPoint.point.position,
                    spawnPoint.point.rotation
                );
            }
            else
            {
                Debug.LogWarning($"Префаб врага {spawnPoint.enemyPrefabName} не найден!");
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Редактор: кнопка для обновления списка врагов
#if UNITY_EDITOR
    [ContextMenu("Обновить список врагов")]
    private void UpdateEnemyList()
    {
        LoadEnemyPrefabs();
        UpdateEnemyNamesList();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}