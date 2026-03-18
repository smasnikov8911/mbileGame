using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public float maxSpeed = 5f;  // Максимальная скорость (при полном отклонении джойстика)
	public float minSpeed = 1f;   // Минимальная скорость (если нужно избежать "слишком медленного" движения)
	public int fastSpeed = 7;
	private float realSpeed;

	public Joystick joystick;
	public Animator animator;
	public AudioClip movementSound;
	public float minAnimationSpeed = 0.4f; // Минимальная скорость анимации
	public float minInputThreshold = 0.1f;  // Порог отклонения джойстика (10%)

	private Rigidbody2D rb;
	private Vector2 movement;
	private AudioSource playerAudioSource;
	private Vector2 lastDirection = Vector2.down;

	public Button runButton; // Ссылка на UI-кнопку бега
	private bool isRunning = false; // Флаг состояния бега

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		playerAudioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
		playerAudioSource.clip = movementSound;
		playerAudioSource.loop = true;

		animator.SetFloat("LastHorizontal", lastDirection.x);
		animator.SetFloat("LastVertical", lastDirection.y);
		realSpeed = maxSpeed;
	}

	private void Update()
	{
		float dirX = joystick.Horizontal;
		float dirY = joystick.Vertical;
		movement = new Vector2(dirX, dirY);

		// Обнуляем движение, если джойстик отклонен меньше порога
		if (movement.magnitude < minInputThreshold)
		{
			movement = Vector2.zero;
		}

		UpdateAnimation(movement);
		HandleMovementSound();
		HandleRunToggle();
		UpdateRunState();
	}

	private void FixedUpdate()
	{
		if (movement.magnitude >= minInputThreshold)
		{
			float currentSpeed = movement.magnitude * realSpeed;
			if (currentSpeed > 0 && currentSpeed < minSpeed)
			{
				currentSpeed = minSpeed;
			}
			rb.velocity = movement.normalized * currentSpeed;
		}
		else
		{
			rb.velocity = Vector2.zero; // Полная остановка
		}
	}

	private void UpdateAnimation(Vector2 movement)
	{
		float moveMagnitude = movement.magnitude;

		if (moveMagnitude < minInputThreshold)
		{
			animator.SetFloat("Speed", 0);
			return;
		}

		float normalizedSpeed = Mathf.InverseLerp(minInputThreshold, 1f, moveMagnitude);
		float animationSpeed = Mathf.Lerp(minAnimationSpeed, 1f, normalizedSpeed);

		animator.SetFloat("Speed", animationSpeed);
		animator.SetFloat("Horizontal", movement.x);
		animator.SetFloat("Vertical", movement.y);

		// Сохраняем направление только при достаточном движении
		lastDirection = movement.normalized;
		animator.SetFloat("LastMoveX", lastDirection.x);
		animator.SetFloat("LastMoveY", lastDirection.y);
	}

	private void HandleMovementSound()
	{
		if (playerAudioSource == null || movementSound == null) return;

		if (movement.magnitude >= minInputThreshold)
		{
			if (!playerAudioSource.isPlaying)
			{
				playerAudioSource.Play();
			}

			// Высота звука шага: 1.5 при беге, иначе 1
			playerAudioSource.pitch = isRunning ? 1.5f : 1f;
		}
		else
		{
			if (playerAudioSource.isPlaying)
			{
				playerAudioSource.Stop();
			}
		}
	}

	private void HandleRunToggle()
	{
		// Если пользователь нажал LeftShift — переключаем состояние бега
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			isRunning = !isRunning;
		}
	}

	private void UpdateRunState()
	{
		if (isRunning)
		{
			animator.SetBool("run", true);
			realSpeed = fastSpeed;
		}
		else
		{
			animator.SetBool("run", false);
			realSpeed = maxSpeed;
		}
	}

	public Vector2 GetLastMoveDirection()
	{
		return lastDirection;
	}

	// Дополнительно: поддержка UI-кнопки (если нужна)
	public void OnRunButtonDown()
	{
		isRunning = true;
	}

	public void OnRunButtonUp()
	{
		isRunning = false;
	}
}
