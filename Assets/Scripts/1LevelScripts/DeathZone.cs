using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone2D : MonoBehaviour
{
    // Ссылку на игрока убрали, он найдется сам через GetComponent
    private AudioClip deathClip;
    private GameObject explosionPrefab;

    void Start()
    {
        // Вернули стандартный звук удара
        deathClip = Resources.Load<AudioClip>("Sounds/FallHit");
        explosionPrefab = Resources.Load<GameObject>("Explosion");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Ищем скрипт игрока на объекте, который влетел в зону
        PlayerJetpack2D player = other.GetComponent<PlayerJetpack2D>();

        // 2. Если это Игрок, используем его метод смерти
        if (player != null)
        {
            // Эта функция вызовет взрыв из 50 частей, звук удара и рестарт
            player.Die(); 
            return; 
        }

        // Запасной вариант для других объектов
        PerformDeathSequence(other.transform.position);
    }

    void PerformDeathSequence(Vector3 position)
    {
        if (explosionPrefab != null)
        {
            GameObject boom = Instantiate(explosionPrefab, position, Quaternion.identity);
            DontDestroyOnLoad(boom); 
        }

        if (deathClip != null)
        {
            GameObject soundObj = new GameObject("DeathSoundTemp");
            AudioSource src = soundObj.AddComponent<AudioSource>();
            src.clip = deathClip;
            src.Play();
            
            DontDestroyOnLoad(soundObj); 
            Destroy(soundObj, deathClip.length); 
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}