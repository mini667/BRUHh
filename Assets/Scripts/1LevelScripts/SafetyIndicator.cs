using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с UI Image

public class SafetyIndicator : MonoBehaviour
{
    [Header("Ссылки")]
    public PlayerJetpack2D player; // Перетащи сюда игрока
    public Image statusImage;      // Перетащи сюда картинку UI (Кружок/Квадрат)

    [Header("Цвета")]
    public Color safeColor = Color.green; // Цвет, когда безопасно
    public Color dangerColor = Color.red; // Цвет, когда разобьешься

    void Update()
    {
        if (player == null || statusImage == null) return;

        // Получаем текущую скорость игрока
        float currentSpeed = player.currentSpeed;
        // Получаем лимит скорости
        float limit = player.maxSafeVelocity;

        // Сравниваем
        if (currentSpeed > limit)
        {
            // Слишком быстро -> Красный
            statusImage.color = dangerColor;
        }
        else
        {
            // Нормально -> Зеленый
            statusImage.color = safeColor;
        }
    }
}