using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

public class TreasureChest : MonoBehaviour
{
    [Header("Настройки сундука")]
    public Animator chestAnimator;
    public string openTrigger = "Open";
    public float openDistance = 2f;

    [Header("Настройки дропа")]
    public List<GameObject> lootPrefabs;
    public int minDropCount = 3;
    public int maxDropCount = 5;
    public float dropForce = 5f;
    public float upwardForce = 3f;
    public float spreadAngle = 45f;

    [Header("Эффекты")]
    public ParticleSystem openEffect;
    public AudioClip openSound;

    private bool isOpened = false;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isOpened && player != null && Vector3.Distance(transform.position, player.position) <= openDistance)
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        if (isOpened || lootPrefabs.Count == 0) return;

        isOpened = true;
        
        // Анимация открытия
        if (chestAnimator != null)
            chestAnimator.SetTrigger(openTrigger);
            chestAnimator.SetBool("isOpened", true);   //added tony

        // Эффекты
        if (openEffect != null)
            openEffect.Play();

        if (openSound != null)
            AudioSource.PlayClipAtPoint(openSound, transform.position);

        // Выбрасываем лут
        StartCoroutine(SpawnLoot());
        // Destroy(gameObject); после того как анимка отыграет и лут выпадет
    }

    private IEnumerator SpawnLoot()
    {
        int dropCount = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < dropCount; i++)
        {
            if (lootPrefabs.Count == 0) yield break;

            GameObject lootPrefab = lootPrefabs[Random.Range(0, lootPrefabs.Count)];
            GameObject loot = Instantiate(lootPrefab, transform.position, Quaternion.identity);

            Vector2 forceDirection = GetRandomDirection();
            yield return StartCoroutine(ApplyPopForce(loot.transform, forceDirection));

            yield return new WaitForSeconds(0.2f);
        }
    }

    private Vector2 GetRandomDirection()
    {
        float randomAngle = Random.Range(-spreadAngle, spreadAngle);
        Vector2 direction = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;
        return direction.normalized;
    }

    private IEnumerator ApplyPopForce(Transform loot, Vector2 direction)
    {
        if (loot == null) yield break; // Защита от уничтоженного объекта

        float duration = 0.5f;
        float elapsed = 0f;
        Vector2 startPos = loot.position;
        Vector2 targetPos = startPos + direction * dropForce;

        while (elapsed < duration && loot != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = Mathf.Sin(t * Mathf.PI) * upwardForce;

            // Дополнительная проверка перед изменением позиции
            if (loot != null)
            {
                loot.position = Vector2.Lerp(startPos, targetPos, t) + Vector2.up * height;
            }
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, openDistance);

        Vector3 forward = transform.up;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, spreadAngle) * forward * 2f);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, -spreadAngle) * forward * 2f);
    }
}