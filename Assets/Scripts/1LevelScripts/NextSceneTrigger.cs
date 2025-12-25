using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Ссылки")]
    public GameObject player;
    public FinishUI finishUI;
    public GameObject PanelUI;

    // Скрытая переменная для хранения звука
    private AudioClip finishClip;

    void Start()
    {
        // ЗАГРУЖАЕМ ЗВУК ЗАРАНЕЕ (Чтобы не было лага при финише)
        // Unity будет искать файл в: Assets/Resources/Sounds/Finish
        finishClip = Resources.Load<AudioClip>("Sounds/Finish");

        if (finishClip == null)
        {
            Debug.LogError("Звук не найден! Убедись, что файл лежит в Assets/Resources/Sounds/Finish");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что вошел именно игрок
        if (other.gameObject == player)
        {
            // 1. Логика финиша
            if (LevelTimer.Instance != null) 
                LevelTimer.Instance.StopTimer();
            
            if (finishUI != null) 
                finishUI.Show();

            // 2. Запускаем звук
            PlaySoundInstant();
        }
    }

    void PlaySoundInstant()
    {
        if (finishClip != null)
        {
            // Создаем временный объект для звука, чтобы он доиграл до конца,
            // даже если скрипт финиша отключится
            GameObject audioObj = new GameObject("FinishSound_Temp");
            AudioSource src = audioObj.AddComponent<AudioSource>();
            
            src.clip = finishClip;
            src.volume = 1f; // Громкость (0.0 - 1.0)
            src.Play();

            // Гарантируем, что звук не прервется при переключении сцен или UI
            DontDestroyOnLoad(audioObj); 
            Destroy(audioObj, finishClip.length); // Удаляем объект, когда звук кончится
        }
    }
}