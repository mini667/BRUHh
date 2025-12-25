using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Ссылка на самого себя

    [Header("Настройки")]
    public AudioSource effectsSource; // Источник для эффектов (взрывы, удары)
    public AudioSource musicSource;   // Источник для музыки (если нужно управлять ей отсюда)

    [Header("Звуки")]
    public AudioClip deathSound;      // Сюда перетащи звук смерти

    void Awake()
    {
        // Паттерн Singleton + DontDestroyOnLoad
        // Это гарантирует, что Менеджер будет жить при смене сцен
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Если случайно создался второй менеджер — убиваем его
        }
    }

    // Метод, который мы будем вызывать из DeathZone
    public void PlayDeathSound()
    {
        // PlayOneShot позволяет играть звуки "поверх" друг друга, не прерывая предыдущие
        effectsSource.PlayOneShot(deathSound);
    }
    
    // Универсальный метод для любых других звуков в будущем
    public void PlaySound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }
}