using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MarsCamera : MonoBehaviour
{
    [Header("Цели")]
    public PlayerJetpack2D playerScript;
    public LayerMask groundLayer;

    [Header("Ограничения Карты")]
    public Transform minLimit; 
    public Transform maxLimit; 

    [Header("Композиция")]
    public float heightOffset = 1f;
    [Tooltip("Насколько сильно смещать камеру в сторону полета")]
    public float lookAheadAmount = 2f; 

    [Header("Настройки Зума")]
    public float minZoom = 6f;
    public float maxZoom = 14f;
    public float zoomSpeed = 2f;    
    public float zoomPadding = 4f; 

    [Header("Плавность")]
    public float searchRadius = 25f; // Радиус поиска
    public float smoothTime = 0.5f;  // Время, за которое камера долетает до цели

    private Camera cam;
    private Vector3 currentVelocity;
    private float fixedZ;

    // Запоминаем платформы
    private Transform currentPlatform; // Где мы сейчас стоим
    private Transform nextPlatform;    // Куда надо лететь

    void Start()
    {
        cam = GetComponent<Camera>();
        fixedZ = transform.position.z;

        if (playerScript == null)
            playerScript = FindFirstObjectByType<PlayerJetpack2D>();
    }

    void LateUpdate()
    {
        if (playerScript == null) return;

        // 1. Обновляем информацию о платформах
        UpdatePlatformsLogick();

        // 2. Рассчитываем, что должно быть в кадре (Bounds)
        Bounds targetBounds = CalculateTargetBounds();

        // 3. ЗУМ: Плавно меняем размер камеры
        float targetSize = CalculateZoom(targetBounds);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        // 4. ПОЗИЦИЯ: Находим центр этой зоны
        Vector3 targetPos = targetBounds.center;
        
        // Добавляем смещение вверх
        targetPos.y += heightOffset;
        targetPos.z = fixedZ;

        // 5. ДВИЖЕНИЕ: Очень плавный полет к точке
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);

        // 6. Ограничиваем рамками уровня
        transform.position = ClampPositionWithinLimits(smoothedPos);
    }

    void UpdatePlatformsLogick()
    {
        // Если игрок стоит на платформе (переменная из PlayerJetpack2D)
        if (playerScript.CurrentPlatform != null)
        {
            currentPlatform = playerScript.CurrentPlatform;
            // Ищем следующую платформу справа от текущей
            nextPlatform = FindNextPlatform(currentPlatform);
        }
        else
        {
            // Если игрок летит, мы не меняем currentPlatform и nextPlatform, 
            // чтобы камера не дергалась, а продолжала показывать цель.
            // Но если nextPlatform еще нет (старт игры), пробуем найти ближайшую
            if (nextPlatform == null)
            {
                nextPlatform = FindNextPlatform(playerScript.transform);
            }
        }
    }

    Transform FindNextPlatform(Transform origin)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin.position, searchRadius, groundLayer);
        
        Transform bestNext = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            // Ищем платформу, которая правее текущей точки (X > origin.x)
            // Добавляем +0.5f, чтобы не найти саму себя
            if (hit.transform.position.x > origin.position.x + 0.5f)
            {
                float dist = Vector2.Distance(origin.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    bestNext = hit.transform;
                }
            }
        }
        return bestNext;
    }

    Bounds CalculateTargetBounds()
    {
        Bounds b;

        // ЛОГИКА КАМЕРЫ:
        
        // СИТУАЦИЯ 1: Игрок стоит на платформе
        if (playerScript.CurrentPlatform != null && currentPlatform != null)
        {
            // Фокусируемся на ТЕКУЩЕЙ и СЛЕДУЮЩЕЙ платформе.
            // Игрок тут не важен (он все равно на платформе).
            b = new Bounds(currentPlatform.position, Vector3.zero);
            
            if (nextPlatform != null)
            {
                b.Encapsulate(nextPlatform.position);
            }
            else
            {
                // Если следующей нет (конец уровня), просто смотрим на игрока
                b.Encapsulate(playerScript.transform.position);
            }
        }
        // СИТУАЦИЯ 2: Игрок летит
        else
        {
            // Фокусируемся на ИГРОКЕ и ЦЕЛИ (NextPlatform)
            b = new Bounds(playerScript.transform.position, Vector3.zero);
            
            // Чуть-чуть сдвигаем фокус вперед по полету, чтобы видеть, куда летим
            Vector3 lookAhead = Vector3.right * lookAheadAmount;
            b.Encapsulate(playerScript.transform.position + lookAhead);

            if (nextPlatform != null)
            {
                b.Encapsulate(nextPlatform.position);
            }
        }

        return b;
    }

    float CalculateZoom(Bounds b)
    {
        // Рассчитываем необходимый зум, чтобы вместить коробку Bounds
        float screenRatio = cam.aspect;
        float targetHeight = b.size.y + zoomPadding;
        float targetWidth = (b.size.x + zoomPadding) / screenRatio;

        float neededSize = Mathf.Max(targetHeight, targetWidth) * 0.5f;

        return Mathf.Clamp(neededSize, minZoom, maxZoom);
    }

    Vector3 ClampPositionWithinLimits(Vector3 target)
    {
        if (minLimit == null || maxLimit == null) return target;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float minX = minLimit.position.x + horzExtent;
        float maxX = maxLimit.position.x - horzExtent;
        float minY = minLimit.position.y + vertExtent;
        float maxY = maxLimit.position.y - vertExtent;

        // Если уровень меньше экрана, центруем
        float clampedX = (minX > maxX) ? (minLimit.position.x + maxLimit.position.x) / 2f : Mathf.Clamp(target.x, minX, maxX);
        float clampedY = (minY > maxY) ? (minLimit.position.y + maxLimit.position.y) / 2f : Mathf.Clamp(target.y, minY, maxY);

        return new Vector3(clampedX, clampedY, target.z);
    }

    void OnDrawGizmos()
    {
        // Рисуем границы поиска для отладки
        Gizmos.color = Color.cyan;
        if(playerScript != null)
            Gizmos.DrawWireSphere(playerScript.transform.position, searchRadius);
    }
}