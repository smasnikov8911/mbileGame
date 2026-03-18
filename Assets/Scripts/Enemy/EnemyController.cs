using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;
    public float losePlayerRadius = 8f;
    public float attackRadius = 1.5f;

    [Header("References")]
    public LayerMask obstacleLayers;
    public EdgeCollider2D patrolArea;
    private Transform playerTransform;
    private Rigidbody2D rb;

    public Animator animator;
    public Animator animatorAttack;
    public SpriteRenderer spriteVisual;
    public SpriteRenderer spriteAttack;

    private EnemyStateMachine stateMachine;
    private Vector2 movementDirection;
    private Vector2 targetPosition;
    private float patrolTimer;
    private float patrolWaitTime;
    private float attackCooldown;
    private float attackTimer;

    private enum AvoidDirection { None, Left, Right }
    private AvoidDirection currentAvoidDirection = AvoidDirection.None;
    private float avoidDuration = 1.5f;
    private float avoidTimer = 0f;
    private const float raycastLength = 3f;


    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Idle
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new EnemyStateMachine();
        stateMachine.Initialize(EnemyState.Patrol);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        patrolWaitTime = Random.Range(1f, 3f);
        attackCooldown = 1.5f;
        SetRandomPatrolTarget();
    }

    private void Update()
    {
        patrolTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        avoidTimer -= Time.deltaTime;

        UpdateState();

        switch (stateMachine.CurrentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
            case EnemyState.Idle:
                Idle();
                break;
        }

        UpdateAnimation();
    }


    private void FixedUpdate()
    {
        if (stateMachine.CurrentState == EnemyState.Patrol || stateMachine.CurrentState == EnemyState.Chase)
        {
            float currentSpeed = stateMachine.CurrentState == EnemyState.Patrol ? patrolSpeed : chaseSpeed;
            rb.velocity = movementDirection * currentSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateState()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius && stateMachine.CurrentState != EnemyState.Chase && stateMachine.CurrentState != EnemyState.Attack)
        {
            stateMachine.ChangeState(EnemyState.Chase);
        }
        else if (distanceToPlayer <= attackRadius && stateMachine.CurrentState == EnemyState.Chase)
        {
            stateMachine.ChangeState(EnemyState.Attack);
        }
        else if (distanceToPlayer > losePlayerRadius && stateMachine.CurrentState == EnemyState.Chase)
        {
            stateMachine.ChangeState(EnemyState.Patrol);
            SetRandomPatrolTarget();
        }
        else if (distanceToPlayer > attackRadius && stateMachine.CurrentState == EnemyState.Attack)
        {
            stateMachine.ChangeState(EnemyState.Chase);
        }
    }


    private void Patrol()
    {
        animator.enabled = true;
        // Если достигли целевой точки или истекло время патрулирования
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f || patrolTimer <= 0)
        {
            stateMachine.ChangeState(EnemyState.Idle);
            rb.velocity = Vector2.zero;
            patrolTimer = patrolWaitTime;

            Invoke("SetRandomPatrolTarget", patrolWaitTime);
            Invoke("ResumePatrol", patrolWaitTime);
        }
        else
        {
            Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayers);
            Debug.DrawRay(transform.position, direction * 1f, Color.red);

            if (hit.collider == null)
            {
                movementDirection = direction;
            }
            else
            {
                Vector2 left = Vector2.Perpendicular(direction);
                RaycastHit2D leftHit = Physics2D.Raycast(transform.position, left, 1f, obstacleLayers);
                if (leftHit.collider == null)
                {
                    movementDirection = left;
                }
                else
                {
                    Vector2 right = -left;
                    RaycastHit2D rightHit = Physics2D.Raycast(transform.position, right, 1f, obstacleLayers);
                    if (rightHit.collider == null)
                    {
                        movementDirection = right;
                    }
                    else
                    {
                        movementDirection = Vector2.zero;
                    }
                }
            }
        }
    }



    private void ChasePlayer()
    {
        animator.enabled = true;
        if (playerTransform == null) return;

        Vector2 toPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, raycastLength, obstacleLayers);
        Debug.DrawRay(transform.position, toPlayer * raycastLength, Color.red);

        if (hit.collider == null && avoidTimer <= 0)
        {
            // Прямой путь свободен — сбрасываем обход
            movementDirection = toPlayer;
            currentAvoidDirection = AvoidDirection.None;
        }
        else
        {
            // Есть препятствие или активен обход
            if (avoidTimer <= 0)
            {
                avoidTimer = avoidDuration;

                if (hit.collider != null)
                {
                    Vector2 normal = hit.normal;
                    Vector2 tangent = new Vector2(-normal.y, normal.x).normalized;

                    // Выбираем направление обхода только один раз
                    Vector2 optionA = tangent;
                    Vector2 optionB = -tangent;

                    float distA = Vector2.Distance((Vector2)transform.position + optionA, playerTransform.position);
                    float distB = Vector2.Distance((Vector2)transform.position + optionB, playerTransform.position);

                    currentAvoidDirection = distA < distB ? AvoidDirection.Left : AvoidDirection.Right;
                }
                else
                {
                    // fallback, если что-то пошло не так
                    currentAvoidDirection = AvoidDirection.Left;
                }
            }

            // продолжаем двигаться вдоль стены
            if (currentAvoidDirection != AvoidDirection.None)
            {
                Vector2 avoidDirection = currentAvoidDirection == AvoidDirection.Left
                    ? Vector2.Perpendicular(toPlayer).normalized
                    : -Vector2.Perpendicular(toPlayer).normalized;

                // проверяем, не упираемся ли прямо сейчас в новое препятствие
                RaycastHit2D sideHit = Physics2D.Raycast(transform.position, avoidDirection, raycastLength, obstacleLayers);
                Debug.DrawRay(transform.position, avoidDirection * raycastLength, Color.green);

                if (sideHit.collider == null)
                {
                    movementDirection = avoidDirection;
                }
                else
                {
                    // если и вбок заблокировано — стоим
                    movementDirection = Vector2.zero;
                }

                avoidTimer -= Time.deltaTime;
                if (avoidTimer <= 0)
                {
                    currentAvoidDirection = AvoidDirection.None;
                }
            }
            else
            {
                movementDirection = Vector2.zero;
            }
        }
    }




    private void AttackPlayer()
    {
        if (attackTimer <= 0)
        {
            spriteVisual.enabled = false;
            spriteAttack.enabled = true;

            Invoke("EnableSpriteVisual", 0.5f);

            TryDamagePlayer();

            attackTimer = attackCooldown;
        }
    }

    private void EnableSpriteVisual()
    {
        spriteVisual.enabled = true;
        spriteAttack.enabled = false;
    }

    private void Idle()
    {
        rb.velocity = Vector2.zero;
        //animator.SetFloat("moveX1", 0f);
        //animator.SetFloat("moveY1", -1f);
        //animator.SetFloat("speed", 0.01f);
        animator.enabled = false;
    }

    private void SetRandomPatrolTarget()
    {
        if (patrolArea == null)
        {
            Debug.LogWarning("Patrol area not assigned!");
            return;
        }

        // Получаем границы области патруля
        Bounds bounds = patrolArea.bounds;

        int attempts = 0;
        const int maxAttempts = 10;
        Vector2 newTarget = transform.position;

        // Пробуем найти точку, не попадающую в препятствия
        while (attempts < maxAttempts)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            newTarget = new Vector2(randomX, randomY);

            // Проверяем, свободна ли точка от препятствий (с помощью Raycast вниз)
            RaycastHit2D hit = Physics2D.CircleCast(newTarget, 0.3f, Vector2.zero, 0f, obstacleLayers);
            if (hit.collider == null)
                break;

            attempts++;
        }

        targetPosition = newTarget;
        patrolTimer = Random.Range(2f, 5f);
    }

    private void ResumePatrol()
    {
        if (stateMachine.CurrentState == EnemyState.Idle)
        {
            stateMachine.ChangeState(EnemyState.Patrol);
        }
    }

    private void TryDamagePlayer()
    {
        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= attackRadius)
        {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                playerHealth.UpdateHealthBar();
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        Vector2 velocity = rb.velocity;
        bool isMoving = velocity.magnitude > 0.1f;

        if (isMoving)
        {
            Vector2 direction = velocity.normalized;
            if (animatorAttack != null)
            {
                animatorAttack.SetFloat("moveX1", direction.x);
                animatorAttack.SetFloat("moveY1", direction.y);
                animatorAttack.SetFloat("speed", 0.75f);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: animatorAttack не назначен!");
            }

            animator.SetFloat("moveX1", direction.x);
            animator.SetFloat("moveY1", direction.y);
            animator.SetFloat("speed", velocity.magnitude);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, losePlayerRadius);
    }
}