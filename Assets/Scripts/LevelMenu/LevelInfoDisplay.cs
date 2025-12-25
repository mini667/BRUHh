using UnityEngine;
using TMPro;
using System;

public class LevelMenuManager : MonoBehaviour
{
    [Serializable]
    public class LevelUIEntry
    {
        public int levelIndex;           // Номер уровня (как в FinishUI)
        public TextMeshProUGUI timeText;  // Текст времени над кнопкой
        public TextMeshProUGUI starsText; // Текст звезд над кнопкой
    }

    [Header("Настройки отображения уровней")]
    public LevelUIEntry[] levels; 

    void Start()
    {
        UpdateMenuVisuals();
    }

    public void UpdateMenuVisuals()
    {
        foreach (LevelUIEntry level in levels)
        {
            // Формируем ключи для поиска данных (такие же, как в твоем FinishUI)
            string timeKey = "Level_" + level.levelIndex + "_Time";
            string starKey = "Level_" + level.levelIndex + "_Stars";

            // 1. ПРОВЕРКА ВРЕМЕНИ
            if (PlayerPrefs.HasKey(timeKey))
            {
                // Если данные есть — показываем результат
                float savedTime = PlayerPrefs.GetFloat(timeKey);
                level.timeText.text = FormatTime(savedTime);
            }
            else
            {
                // Если данных нет — "онилируем" (ставим прочерки или нули)
                level.timeText.text = "--:--"; 
            }

            // 2. ПРОВЕРКА ЗВЕЗД
            if (level.starsText != null)
            {
                if (PlayerPrefs.HasKey(starKey))
                {
                    int savedStars = PlayerPrefs.GetInt(starKey);
                    level.starsText.text = "Звёзд: " + savedStars;
                }
                else
                {
                    // "Онилируем" звезды
                    level.starsText.text = "Звёзд: 0";
                }
            }
        }
    }

    // Вспомогательный метод для красивого времени
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return string.Format("{0:0}:{1:00}", minutes, secs);
    }
}