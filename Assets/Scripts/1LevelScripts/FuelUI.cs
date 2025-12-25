using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [Header("Ссылки")]
    public PlayerJetpack2D player; // Перетащи сюда игрока
    public Image fuelBar;          // Перетащи сюда саму картинку топлива
    
    [Header("Позиция")]
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Насколько выше головы будет висеть бар

    void Start()
    {
        // Если забыли привязать игрока вручную, пробуем найти сами
        if (player == null)
            player = FindFirstObjectByType<PlayerJetpack2D>();
    }

    void Update()
    {
        if (player == null) return;

        // --- 1. ЛОГИКА ЗАПОЛНЕНИЯ ---
        if (fuelBar != null)
        {
            // Считаем процент топлива (от 0 до 1)
            fuelBar.fillAmount = player.fuel / player.maxFuel;
        }

        // --- 2. ЛОГИКА СЛЕЖЕНИЯ ---
        // Берем позицию игрока + смещение (offset) и переводим в координаты экрана
        if (Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position + offset);
            transform.position = screenPos;
        }
    }
}