using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    [Header("Настройки Звука")]
    public AudioClip soundToPlay; // Перетащи сюда свой 2-секундный файл
    [Range(0f, 1f)]
    public float volume = 1f;     // Громкость (1 = максимум)
    
    public bool destroyAfterSound = false; // Если это монетка, поставь галочку (объект исчезнет)

    private bool hasPlayed = false; // Защита от двойного срабатывания

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что коснулся именно Игрок, а не пол или другой объект
        if (hasPlayed) return;

        if (other.GetComponent<PlayerJetpack2D>() != null)
        {
            PlaySound();
        }
    }

    void PlaySound()
    {
        hasPlayed = true;

        if (SoundManager.instance != null && soundToPlay != null)
        {
            // Используем метод PlayOneShot, чтобы звуки накладывались, а не прерывали друг друга
            SoundManager.instance.effectsSource.PlayOneShot(soundToPlay, volume);
        }

        if (destroyAfterSound)
        {
            // Скрываем объект (визуально), но даем скрипту доработать кадр (хотя для звука через Менеджер это не критично)
            Destroy(gameObject);
        }
    }
}