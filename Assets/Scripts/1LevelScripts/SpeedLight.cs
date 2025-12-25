using UnityEngine;

public class SpeedFlags : MonoBehaviour
{
    [Header("Ссылки")]
    public PlayerJetpack2D playerScript;   // Ссылка на игрока
    
    [Header("Объекты Флагов (Pivot)")]
    public Transform greenFlagPivot;       // Пустой объект, внутри которого лежит зеленый флаг
    public Transform redFlagPivot;         // Пустой объект, внутри которого лежит красный флаг

    [Header("Настройки")]
    public float rotationSpeed = 10f;      // Плавность поворота флажков

    private Rigidbody2D rb;

    void Start()
    {
        // Автоматически находим игрока, если забыли перетащить
        if (playerScript == null)
            playerScript = FindFirstObjectByType<PlayerJetpack2D>();

        if (playerScript != null)
            rb = playerScript.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerScript == null || rb == null || greenFlagPivot == null || redFlagPivot == null) return;

        // 1. Получаем скорость (поддержка Unity 6 и старых версий)
        float currentSpeed = rb.linearVelocity.magnitude; 

        // 2. Проверяем, безопасна ли скорость
        bool isSafe = currentSpeed <= playerScript.maxSafeVelocity;

        // 3. Определяем целевые углы поворота по оси X
        // Если безопасно: Зеленый = 0 (стоит), Красный = -90 (лежит)
        // Если опасно:    Зеленый = -90 (лежит), Красный = 0 (стоит)
        float targetGreenX = isSafe ? 0f : -90f;
        float targetRedX   = isSafe ? -90f : 0f;

        // 4. Плавно вращаем флажки (используем localRotation, чтобы не зависеть от поворота самой платформы)
        Quaternion greenTargetRot = Quaternion.Euler(targetGreenX, 0, 0);
        Quaternion redTargetRot   = Quaternion.Euler(targetRedX, 0, 0);

        greenFlagPivot.localRotation = Quaternion.Lerp(greenFlagPivot.localRotation, greenTargetRot, Time.deltaTime * rotationSpeed);
        redFlagPivot.localRotation = Quaternion.Lerp(redFlagPivot.localRotation, redTargetRot, Time.deltaTime * rotationSpeed);
    }
}