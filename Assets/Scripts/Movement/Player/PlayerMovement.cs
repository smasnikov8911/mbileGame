using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 5f; // Скорость движения
	private Animator animator; // Ссылка на компонент Animator
	private Vector2 movement; // Вектор движения
	public Joystick joystick; // Ссылка на джойстик
	private Rigidbody2D rb;

	[Header("Sound Settings")]
	public AudioSource footstepAudioSource; // Источник звука шагов
	public AudioClip[] footstepSounds; // Массив звуков шагов
	public float footstepDelay = 0.4f; // Задержка между шагами
	private float nextFootstepTime; // Время следующего шага

	void Start()
	{
		// Получаем компонент Animator
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();

		// Проверяем наличие AudioSource
		if (footstepAudioSource == null)
		{
			footstepAudioSource = gameObject.AddComponent<AudioSource>();
		}
	}

	void Update()
	{
		// Получение ввода с джойстика
		float dirX = joystick.Horizontal;
		float dirY = joystick.Vertical;

		// Получение ввода с клавиатуры (WASD)
		if (Input.GetKey(KeyCode.W)) dirY = 1f;
		if (Input.GetKey(KeyCode.S)) dirY = -1f;
		if (Input.GetKey(KeyCode.A)) dirX = -1f;
		if (Input.GetKey(KeyCode.D)) dirX = 1f;

		// Нормализация вектора движения
		movement = new Vector2(dirX, dirY).normalized;

		// Управление анимацией
		UpdateAnimation(movement);

		// Воспроизведение звуков шагов
		HandleFootsteps(movement);
	}

	private void FixedUpdate()
	{
		rb.velocity = movement * speed;
	}

	private void UpdateAnimation(Vector2 movement)
	{
		// Сначала сбрасываем все анимации
		animator.SetBool("UpRun", false);
		animator.SetBool("DownRun", false);
		animator.SetBool("LeftRun", false);
		animator.SetBool("RightRun", false);

		if (movement.magnitude > 0)
		{
			animator.SetBool("isWalking", true);

			// Определение направления
			if (movement.x > 0 && movement.y > 0) // Вправо-вверх
			{
				animator.SetBool("UpRun", true);
				animator.SetBool("RightRun", true);
			}
			else if (movement.x > 0 && movement.y < 0) // Вправо-вниз
			{
				animator.SetBool("DownRun", true);
				animator.SetBool("RightRun", true);
			}
			else if (movement.x < 0 && movement.y > 0) // Влево-вверх
			{
				animator.SetBool("UpRun", true);
				animator.SetBool("LeftRun", true);
			}
			else if (movement.x < 0 && movement.y < 0) // Влево-вниз
			{
				animator.SetBool("DownRun", true);
				animator.SetBool("LeftRun", true);
			}
			else if (movement.x > 0) // Вправо
			{
				animator.SetBool("RightRun", true);
			}
			else if (movement.x < 0) // Влево
			{
				animator.SetBool("LeftRun", true);
			}
			else if (movement.y > 0) // Вверх
			{
				animator.SetBool("UpRun", true);
			}
			else if (movement.y < 0) // Вниз
			{
				animator.SetBool("DownRun", true);
			}
		}
		else
		{
			animator.SetBool("isWalking", false);

			// Сбрасываем все направления в состояние покоя
			animator.SetBool("UpRun", false);
			animator.SetBool("DownRun", false);
			animator.SetBool("LeftRun", false);
			animator.SetBool("RightRun", false);
		}
	}

	private void HandleFootsteps(Vector2 movement)
	{
		if (movement.magnitude > 0.1f) // Если персонаж движется
		{
			if (Time.time >= nextFootstepTime)
			{
				PlayFootstepSound();
				nextFootstepTime = Time.time + footstepDelay;
			}
		}
	}

	private void PlayFootstepSound()
	{
		if (footstepSounds.Length > 0 && footstepAudioSource != null)
		{
			// Выбираем случайный звук шага из массива
			int index = Random.Range(0, footstepSounds.Length);
			footstepAudioSource.clip = footstepSounds[index];
			footstepAudioSource.Play();
		}
	}
}
