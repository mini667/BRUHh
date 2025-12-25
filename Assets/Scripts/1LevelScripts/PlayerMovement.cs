using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJetpack2D : MonoBehaviour
{
    [Header("Mars Physics")]
    public float thrustForce = 25f;      
    public float verticalPower = 1.3f;   
    public float tiltAngle = 20f;        
    public float tiltSpeed = 5f;         

    [Header("Смерть и Эффекты")]
    public float maxSafeVelocity = 8f;   
    private bool isDead = false;
    
    // --- НАСТРОЙКИ ВЗРЫВА ---
    [Header("Настройки Взрыва (Скриптом)")]
    public GameObject[] explosionParts; // Сюда перетащи префабы осколков
    public int partsCount = 50;         // Сколько кусков вылетит
    public float explosionForce = 15f;  // Сила разлета
    public float partMinSize = 0.2f;    // Мин размер осколка
    public float partMaxSize = 0.6f;    // Макс размер осколка
    
    // Ресурсы
    private AudioClip deathClip;

    [Header("Debug Info")]
    public float currentSpeed; 

    [Header("Platform Settings")]
    public float centeringSpeed = 3f;    
    public float stickCooldown = 0.3f;   
    public float launchForce = 15f;      
    [Range(0, 90)]
    public float launchAngle = 30f;      

    [Header("Fuel")]
    public float maxFuel = 100f;
    public float fuel;
    public float fuelConsumePerSecond = 20f;

    public Transform CurrentPlatform; 
    public bool IsGrounded => isOnPlatform || isCentering;

    public Rigidbody2D rb { get; private set; }
    private Animator anim;                 
    private JetpackBulletExhaust effects;  
    private SpriteRenderer spriteRenderer; 

    private bool isOnPlatform = true;
    private bool isCentering = false;
    private Vector3 targetCenterPos;
    private float lastLaunchTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();            
        effects = GetComponent<JetpackBulletExhaust>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        fuel = maxFuel;
        
        rb.gravityScale = 1.6f;
        // Для Unity 6 используем linearDamping, для старых - drag
        rb.linearDamping = 1.1f; 
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Авто-загрузка звука (должен лежать в папке Resources/Sounds)
        deathClip = Resources.Load<AudioClip>("Sounds/FallHit");
        
        if (isOnPlatform) StickToPlatform();
    }

    void Update()
    {
        if (isDead) return;

        // В Unity 6 используется linearVelocity, в старых - velocity
        currentSpeed = rb.linearVelocity.magnitude;

        if (anim != null)
        {
            // Передаем аниматору: стоим мы на земле или нет
            anim.SetBool("IsOnPlatform", isOnPlatform || isCentering);
        }

        // Логика прыжка
        if ((isOnPlatform || isCentering) && Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }
    }

    void Launch()
    {
        // Если аниматор есть, сообщаем ему: "ПРЫГАЙ!"
        if (anim != null) 
        {
            anim.SetTrigger("Jump");
        }

        isCentering = false;
        isOnPlatform = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        lastLaunchTime = Time.time;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        Vector2 launchDir = new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad));
        
        // Применяем скорость запуска
        rb.linearVelocity = launchDir * launchForce;

        if (effects != null) effects.LaunchBurst();
    }

    void FixedUpdate()
    {
        if (isDead || isOnPlatform || isCentering) return;
        HandleJetpackPhysics();
        HandleRotation();
    }

    void HandleJetpackPhysics()
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        if (fuel <= 0 || (!left && !right)) return;

        Vector2 forceDirection = Vector2.zero;
        if (left && right) forceDirection = Vector2.up * verticalPower;
        else if (right) forceDirection = new Vector2(-1f, 0.6f); 
        else if (left) forceDirection = new Vector2(1f, 0.6f);

        rb.AddForce(forceDirection * thrustForce);
        fuel -= fuelConsumePerSecond * Time.fixedDeltaTime;
        fuel = Mathf.Clamp(fuel, 0, maxFuel);
    }

    void HandleRotation()
    {
        float targetZ = 0;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) targetZ = tiltAngle;
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) targetZ = -tiltAngle;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * tiltSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // Смертельная скорость
        if (collision.relativeVelocity.magnitude > maxSafeVelocity)
        {
            Die(); 
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CurrentPlatform = collision.transform;

            if (Time.time - lastLaunchTime > stickCooldown)
            {
                float platformTop = collision.collider.bounds.max.y;
                float playerHalfHeight = GetComponent<Collider2D>().bounds.extents.y;
                
                targetCenterPos = new Vector3(collision.transform.position.x, platformTop + playerHalfHeight, transform.position.z);
                isCentering = true;
                StickToPlatform();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == CurrentPlatform)
        {
            CurrentPlatform = null; 
        }
    }

    void StickToPlatform()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; 
        fuel = maxFuel;
        transform.rotation = Quaternion.identity;
    }

    // --- ПУБЛИЧНЫЙ МЕТОД DIE ---
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. ЗАПУСКАЕМ ВЗРЫВ
        CreateExplosion();

        // 2. Звук
        if (deathClip != null)
        {
            GameObject audioObj = new GameObject("TempDeathSound");
            AudioSource src = audioObj.AddComponent<AudioSource>();
            src.clip = deathClip;
            src.volume = 1f; 
            src.Play();
            
            DontDestroyOnLoad(audioObj); 
            Destroy(audioObj, deathClip.length); 
        }

        // 3. Скрываем игрока
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 4. Рестарт уровня
        RestartLevel();
    }

    // --- СОЗДАНИЕ ВЗРЫВА ИЗ ПРЕФАБОВ ---
    void CreateExplosion()
    {
        if (explosionParts == null || explosionParts.Length == 0) return;

        // Создаем контейнер, который переживет перезагрузку сцены
        GameObject debrisContainer = new GameObject("ExplosionDebris");
        DontDestroyOnLoad(debrisContainer); 
        Destroy(debrisContainer, 3f); // Удаляем контейнер через 3 сек

        for (int i = 0; i < partsCount; i++)
        {
            GameObject prefab = explosionParts[Random.Range(0, explosionParts.Length)];
            GameObject part = Instantiate(prefab, transform.position, Quaternion.identity);
            
            part.transform.SetParent(debrisContainer.transform);
            part.transform.localScale = Vector3.one * Random.Range(partMinSize, partMaxSize);

            Rigidbody2D partRb = part.GetComponent<Rigidbody2D>();
            if (partRb != null)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                partRb.linearVelocity = dir * Random.Range(explosionForce * 0.5f, explosionForce * 1.5f);
                partRb.angularVelocity = Random.Range(-720f, 720f);
            }
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LateUpdate()
    {
        if (isCentering)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetCenterPos, centeringSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetCenterPos) < 0.05f)
            {
                isCentering = false;
                isOnPlatform = true;
            }
        }
    }

    public void AddFuel(float amount)
    {
        fuel += amount;
        fuel = Mathf.Clamp(fuel, 0, maxFuel);
    }
}