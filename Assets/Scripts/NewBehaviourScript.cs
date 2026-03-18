using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public Slider staminaSlider;  // Перетащите сюда ваш Slider из редактора
    public float maxStamina = 10f;  // Макс. запас стамины
    public float currentStamina;  // Текущая стамина
    public float staminaDepletionRate = 1f;  // Скорость траты стамины (за 1 сек)
    public float staminaRegenRate = 0.5f;  // Скорость восстановления (за 1 сек)

    private bool isRunning = false;

    private void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    private void Update()
    {
        if (isRunning)
        {
            // Тратим стамину, если бежим
            currentStamina -= staminaDepletionRate * Time.deltaTime;

            // Если стамина закончилась, останавливаем бег
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                StopRunning();
            }
        }
        else if (currentStamina < maxStamina)
        {
            // Восстанавливаем стамину, если не бежим
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        // Обновляем UI
        staminaSlider.value = currentStamina;
    }

    // Вызывается при нажатии кнопки RunButton
    public void StartRunning()
    {
        if (currentStamina > 0)
        {
            isRunning = true;
            // Здесь можно добавить увеличение скорости персонажа
        }
    }

    // Вызывается, когда стамина закончилась или игрок отпустил кнопку
    public void StopRunning()
    {
        isRunning = false;
        // Здесь можно сбросить скорость персонажа
    }
}