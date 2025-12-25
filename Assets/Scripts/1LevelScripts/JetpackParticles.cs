using UnityEngine;

public class JetpackBulletExhaust : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public GameObject[] debrisPrefabs; 
    public Transform leftSpawnPoint;   
    public Transform rightSpawnPoint;  

    [Header("Настройки пули (A/D)")]
    public float bulletSpeed = 15f;    
    public float spawnRate = 0.02f;    
    public float minAngle = 30f;       
    public float maxAngle = 45f;       

    [Header("Настройки взлета (Space)")]
    public int burstCount = 15;        
    public float burstSpeedMultiplier = 1.2f; 

    [Header("Визуал и Жизнь")]
    public float minScale = 0.05f;     
    public float maxScale = 0.2f;      
    public float rotationSpeed = 600f; 
    public float lifetime = 1.2f;      

    private float nextSpawnTime;
    private PlayerJetpack2D playerScript;
    private Rigidbody2D rb; // Ссылка на свой же Rigidbody

    void Start()
    {
        playerScript = GetComponent<PlayerJetpack2D>();
        rb = GetComponent<Rigidbody2D>(); // Получаем компонент физики
    }

    void Update()
    {
        // ЖЕЛЕЗОБЕТОННАЯ ПРОВЕРКА:
        // 1. Если скрипта нет или топлива нет — стоп.
        // 2. Если физическое тело KINEMATIC — значит мы стоим на платформе — СТОП.
        if (playerScript == null || playerScript.fuel <= 0 || rb.bodyType == RigidbodyType2D.Kinematic) return;

        bool pressingA = Input.GetKey(KeyCode.A);
        bool pressingD = Input.GetKey(KeyCode.D);

        if (Time.time >= nextSpawnTime)
        {
            if (pressingD) 
            { 
                ShootDebris(rightSpawnPoint, 1f, bulletSpeed); 
                nextSpawnTime = Time.time + spawnRate; 
            }
            
            if (pressingA) 
            { 
                ShootDebris(leftSpawnPoint, -1f, bulletSpeed); 
                nextSpawnTime = Time.time + spawnRate; 
            }
        }
    }

    public void LaunchBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            ShootDebris(leftSpawnPoint, -1f, bulletSpeed * burstSpeedMultiplier);
            ShootDebris(rightSpawnPoint, 1f, bulletSpeed * burstSpeedMultiplier);
        }
    }

    void ShootDebris(Transform spawnPoint, float side, float speed)
    {
        if (debrisPrefabs.Length == 0 || spawnPoint == null) return;

        GameObject prefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Length)];
        GameObject bullet = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        bullet.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float angle = Random.Range(minAngle, maxAngle) * side;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.down;
            bulletRb.linearVelocity = direction * speed;
            bulletRb.angularVelocity = Random.Range(-rotationSpeed, rotationSpeed);
        }

        Destroy(bullet, lifetime);
    }
}