using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform[] cameraPositions; // Массив точек для камеры
    private int currentTargetIndex = 0; // Индекс текущей цели камеры

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, если персонаж зашел в триггер
        if (other.CompareTag("Player"))
        {
            // Перемещаем камеру на новую позицию
            MoveCameraToNextPosition();
        }
    }

    void MoveCameraToNextPosition()
    {
        // Проверяем, чтобы индекс не выходил за пределы массива
        if (cameraPositions.Length > 0)
        {
            // Переходим к следующей позиции
            currentTargetIndex = (currentTargetIndex + 1) % cameraPositions.Length;
            Vector3 targetPosition = cameraPositions[currentTargetIndex].position;

            // Легкое перемещение камеры
            StartCoroutine(MoveCamera(targetPosition));
        }
    }

    System.Collections.IEnumerator MoveCamera(Vector3 targetPosition)
    {
        float timeToMove = 1.0f; // Время для плавного перемещения
        Vector3 startPosition = cam.transform.position;
        float elapsedTime = 0f;

        // Плавно двигаем камеру
        while (elapsedTime < timeToMove)
        {
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедимся, что камера точно окажется в целевой точке
        cam.transform.position = targetPosition;
    }
}
