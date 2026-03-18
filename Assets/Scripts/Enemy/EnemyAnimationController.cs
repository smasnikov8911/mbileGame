using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        UpdateAnimation();
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;

        // Получаем текущую скорость движения
        Vector2 velocity = rb.velocity;
        bool isMoving = velocity.magnitude > 0.1f;


        if (isMoving)
        {
            // Нормализуем вектор скорости для определения направления
            Vector2 direction = velocity.normalized;

            animator.SetFloat("moveX1", direction.x);
            animator.SetFloat("moveY1", direction.y);
            animator.SetFloat("speed", velocity.magnitude);
        }
    }
}
