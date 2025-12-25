using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // Паттерн Singleton (Одиночка)
        // Проверяем: если музыкальный плеер уже есть, то удаляем этот (новый), чтобы не было эхо.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <--- ГЛАВНАЯ МАГИЯ: Не уничтожать при переходе/рестарте
        }
        else
        {
            Destroy(gameObject);
        }
    }
}