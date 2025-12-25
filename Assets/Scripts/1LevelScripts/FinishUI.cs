using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с классом Button
using UnityEngine.SceneManagement;
using TMPro;

public class FinishUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI starText;
    
    [Header("Scene Settings")]
    public string nextLevelScene; // Это поле уже позволяет писать название в инспекторе
    public string levelMenuScene;
    public int levelIndex;

    [Header("Buttons")]
    // Добавляем ссылки на сами кнопки
    public Button nextLevelButton;
    public Button menuButton;

    [Header("In-game timer reference")]
    public GameObject inGameTimer;

    
    void Awake() // Используем Awake вместо Start
    {
    // Скрываем панель при запуске на всякий случай, 
    // если забыли выключить в инспекторе
    gameObject.SetActive(false); 

    // Назначаем функции кнопкам
    if (nextLevelButton != null)
        nextLevelButton.onClick.AddListener(NextLevel);

    if (menuButton != null)
        nextLevelButton.onClick.AddListener(LevelMenu);
    }

    void Start()
    {
        // Назначаем функции кнопкам прямо из кода
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(NextLevel);

        if (menuButton != null)
            menuButton.onClick.AddListener(LevelMenu);
    }

    public void Show()
    {
        if (inGameTimer != null)
            inGameTimer.SetActive(false);

        float time = LevelTimer.Instance.GetTime();
        int stars = StarManager.Instance.GetStars();

        timeText.text = "Вы прошли за: " + time.ToString("F2") + " сек";
        starText.text = "Звёзд собрано: " + stars;

        SaveResult(time, stars);

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void SaveResult(float time, int stars)
    {
        string timeKey = "Level_" + levelIndex + "_Time";
        string starKey = "Level_" + levelIndex + "_Stars";

        if (!PlayerPrefs.HasKey(timeKey) || time < PlayerPrefs.GetFloat(timeKey))
            PlayerPrefs.SetFloat(timeKey, time);

        PlayerPrefs.SetInt(starKey, Mathf.Max(stars, PlayerPrefs.GetInt(starKey, 0)));
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelScene);
    }

    public void LevelMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelMenuScene);
    }
}