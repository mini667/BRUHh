using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JetpackMusic : MonoBehaviour
{
    [Header("Ссылки")]
    public PlayerJetpack2D playerScript; // Ссылка на скрипт игрока
    
    private AudioSource audioSource;
    private bool isMusicPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // --- АВТОМАТИЧЕСКИЕ НАСТРОЙКИ ---
        audioSource.loop = true;          // Зацикливаем
        audioSource.playOnAwake = false;  // Выключаем автостарт
        audioSource.spatialBlend = 0f;    // <--- ВАЖНО: Делаем звук 2D (слышно всегда)

        // Ищем игрока, если забыли перетащить
        if (playerScript == null)
            playerScript = GetComponent<PlayerJetpack2D>();
    }

    void Update()
    {
        if (playerScript == null) return;

        // 1. Нажаты ли кнопки?
        bool inputActive = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        // 2. В воздухе ли мы? (Если CurrentPlatform пустой — значит летим)
        bool isInAir = playerScript.CurrentPlatform == null;

        // ЛОГИКА ВКЛЮЧЕНИЯ
        if (inputActive && isInAir)
        {
            if (!isMusicPlaying)
            {
                ResumeMusic();
            }
        }
        else
        {
            if (isMusicPlaying)
            {
                PauseMusic();
            }
        }
    }

    void ResumeMusic()
    {
        isMusicPlaying = true;
        // Если музыка еще не играла — Play, если была на паузе — UnPause
        if (audioSource.time == 0f && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    void PauseMusic()
    {
        isMusicPlaying = false;
        audioSource.Pause(); // Ставим на паузу, чтобы потом продолжить
    }
}