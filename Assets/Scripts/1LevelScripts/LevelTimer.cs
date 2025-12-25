using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;
    private float time;
    private bool running = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (running)
            time += Time.deltaTime;
    }

    public void StopTimer()
    {
        running = false;
    }

    public float GetTime()
    {
        return time;
    }
}
