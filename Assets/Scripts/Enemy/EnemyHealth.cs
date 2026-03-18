using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    public Animator animatorDeath;
    public SpriteRenderer spriteVisual;
    public SpriteRenderer spriteDeath;
    public SpriteRenderer spriteAttack;

    [Header("Visual Feedback")]
    public float flashDuration = 0.1f;
    private Color originalColor;
    
    private void Start()
    {
        currentHealth = maxHealth;
        if (spriteDeath != null)
        {
            originalColor = spriteDeath.color;
        }
    }
    
    public void TakeDamage(int damageAmount = 1)
    {
        currentHealth -= damageAmount;
        
        // Проверка на смерть
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Можно добавить анимацию смерти, звуковой эффект и т.д.
        Debug.Log($"Enemy {gameObject.name} died");

        // Отключаем все визуальные элементы, кроме анимации
        spriteAttack.enabled = false;
        spriteVisual.enabled = false;
        spriteDeath.enabled = true;

        // Запускаем анимацию смерти
        animatorDeath.SetFloat("moveX1", 1f);
        animatorDeath.SetFloat("moveY1", 1f);
        animatorDeath.SetFloat("speed", 1f);

        // Отключаем все компоненты, кроме Animator
        foreach (var component in GetComponents<MonoBehaviour>())
        {
            Debug.Log(component);
            if (component != animatorDeath && component != spriteDeath)
            {
                component.enabled = false;
            }
        }

        // Запускаем корутину для удаления объекта
        StartCoroutine(DestroyAfterDelay(0.5f));
    }


    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Ждем указанное время
        yield return new WaitForSeconds(delay);

        // Уничтожаем объект
        Destroy(gameObject);
    }
}
