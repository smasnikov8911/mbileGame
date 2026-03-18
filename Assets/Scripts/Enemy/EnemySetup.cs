using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemySetup : MonoBehaviour
{
	public Animator animator;
	public Animator animatorAttack;
	public Animator animatorDeath;
	public SpriteRenderer spriteVisual;
	public SpriteRenderer spriteAttack;
	public SpriteRenderer spriteDeath;
	public LayerMask obstacleLayers;
	public EdgeCollider2D patrolArea;

	[Header("Enemy Settings")]
	public float patrolSpeed = 2f;
	public float chaseSpeed = 3f;
	public float detectionRadius = 5f;
	public float losePlayerRadius = 8f;
	public float attackRadius = 1.5f;
	public int damageAmount = 1;
	public int maxHealth = 3;

	private void Awake()
	{
		// Проверка и добавление необходимых компонентов
		if (GetComponent<Rigidbody2D>() == null)
		{
			Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
			rb.gravityScale = 0f;
			rb.freezeRotation = true;
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			rb.interpolation = RigidbodyInterpolation2D.Interpolate;
		}

		if (GetComponent<CircleCollider2D>() == null && GetComponent<BoxCollider2D>() == null)
		{
			CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
			collider.radius = 0.5f;
			collider.isTrigger = false;
		}

		// Добавление всех необходимых скриптов
		if (GetComponent<EnemyController>() == null)
		{
			EnemyController controller = gameObject.AddComponent<EnemyController>();
			controller.patrolSpeed = patrolSpeed;
			controller.chaseSpeed = chaseSpeed;
			controller.detectionRadius = detectionRadius;
			controller.losePlayerRadius = losePlayerRadius;
			controller.attackRadius = attackRadius;
			controller.animator = animator;
			controller.animatorAttack = animatorAttack;
			controller.spriteVisual = spriteVisual;
			controller.spriteAttack = spriteAttack;
			controller.patrolArea = patrolArea;
			controller.obstacleLayers = obstacleLayers;
		}

		if (GetComponent<EnemyAnimationController>() == null)
		{
			EnemyAnimationController animation = gameObject.AddComponent<EnemyAnimationController>();
			animation.animator = animator;
		}

		if (GetComponent<EnemyHealth>() == null)
		{
			EnemyHealth health = gameObject.AddComponent<EnemyHealth>();
			health.maxHealth = maxHealth;
			health.animatorDeath = animatorDeath;
			health.spriteVisual = spriteVisual;
			health.spriteDeath = spriteDeath;
			health.spriteAttack = spriteAttack;
		}
	}
}