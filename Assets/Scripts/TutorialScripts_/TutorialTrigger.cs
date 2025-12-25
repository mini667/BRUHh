using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("ID сценария из Менеджера")]
    public string sequenceID; // Например "Refill" или "Obstacle"
    public PlayerJetpack2D playerScript; // Ссылка на игрока (перетащить в инспекторе)

    // Если забыли перетащить игрока, ищем сами
    void Start()
    {
        if(playerScript == null) 
            playerScript = FindFirstObjectByType<PlayerJetpack2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что это именно наш игрок, а не пуля или еще что-то
        if (playerScript != null && other.gameObject == playerScript.gameObject)
        {
            TutorialManager.instance.StartSequence(sequenceID);
            // Уничтожаем триггер, чтобы он не сработал второй раз (хотя в менеджере есть проверка isCompleted)
            Destroy(gameObject); 
        }
    }
}