using UnityEngine;

public class ActivateTwoObjects : MonoBehaviour
{
    [Header("Кого ждем")]
    [Tooltip("Перетащи сюда Игрока")]
    public GameObject player;

    [Header("Что включить (перетащи сюда кнопки)")]
    [Tooltip("Первая кнопка или объект")]
    public GameObject firstButton;

    [Tooltip("Вторая кнопка или объект")]
    public GameObject secondButton;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что коснулся именно игрок
        if (other.gameObject == player)
        {
            // Включаем первую кнопку (если она назначена)
            if (firstButton != null)
            {
                firstButton.SetActive(true);
            }

            // Включаем вторую кнопку (если она назначена)
            if (secondButton != null)
            {
                secondButton.SetActive(true);
            }
        }
    }
}