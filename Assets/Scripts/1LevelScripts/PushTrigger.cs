using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Гарантирует, что AudioSource будет на объекте
public class PushTrigger2D_Realistic : MonoBehaviour
{
    [Header("Player Reference")]
    public Rigidbody2D playerRb;       // Сюда перетащи Rigidbody игрока
    public float pushStrength = 5f;    // Сила отталкивания

    private AudioClip sheepClip;
    private AudioSource audioSource;

    void Start()
    {
        // 1. Настраиваем звук один раз при запуске игры
        sheepClip = Resources.Load<AudioClip>("Sounds/Sheep");
        
        // Получаем компонент звука (он добавится автоматически благодаря RequireComponent)
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // Чтобы не орал при старте
        audioSource.clip = sheepClip;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerRb == null) return;

        // Проверка: это точно игрок?
        if (other.attachedRigidbody == playerRb)
        {
            // --- ФИЗИКА (МГНОВЕННО) ---
            // 1. Сбрасываем скорость игрока в ноль, чтобы толчок был четким
            playerRb.linearVelocity = Vector2.zero; 

            // 2. Считаем направление от центра триггера к игроку
            Vector2 pushDirection = (playerRb.position - (Vector2)transform.position).normalized;

            // 3. Применяем импульс (Impulse - это мгновенный удар)
            playerRb.AddForce(pushDirection * pushStrength, ForceMode2D.Impulse);

            // --- ЗВУК ---
            // Проигрываем звук без создания лишних объектов
            if (sheepClip != null)
            {
                audioSource.PlayOneShot(sheepClip);
            }
        }
    }
}