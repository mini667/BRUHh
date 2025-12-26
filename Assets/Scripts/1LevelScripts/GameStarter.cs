using UnityEngine;

public class GameStarter : MonoBehaviour
{
    void Start()
    {
        // ПРИНУДИТЕЛЬНО запускаем время при старте сцены
        Time.timeScale = 1f; 
    }
}