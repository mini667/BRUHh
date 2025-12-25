using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("UI Компоненты")]
    public GameObject tutorialPanel;    
    public TMP_Text tutorialText;           
    public Image tutorialImage;         
    public TMP_Text pressKeyText;      
    public RectTransform arrowRect;     

    [Header("Настройки Стрелки")]
    public float arrowOffset = 100f;    

    // --- СТРУКТУРА ДАННЫХ (БЕЗ ИЗМЕНЕНИЙ) ---
    
    [System.Serializable]
    public class TutorialPage
    {
        [TextArea] public string text;  
        public Sprite screenshot;       
        public RectTransform uiTarget;  
        public Transform worldTarget;   
    }

    [System.Serializable]
    public class TutorialSequence
    {
        public string sequenceID;       
        public TutorialPage[] pages;
        // isCompleted теперь используем только для временной логики внутри кадра
        [HideInInspector] public bool isCompleted; 
    }

    [Header("Все сценарии игры")]
    public TutorialSequence[] allSequences;

    // Внутренние переменные
    private Queue<TutorialPage> currentPagesQueue = new Queue<TutorialPage>();
    private bool isTutorialActive = false;
    private bool hasLaunchedGame = false; 

    void Awake()
    {
        instance = this;
        tutorialPanel.SetActive(false);
        arrowRect.gameObject.SetActive(false);
    }

    void Start()
    {
        // Пытаемся запустить Интро.
        // Если оно уже сохранено в памяти как пройденное, метод StartSequence сам это поймет и отменит показ.
        StartSequence("Intro");
    }

    void Update()
    {
        // 1. ЛОГИКА ТУТОРИАЛА (Нажми любую кнопку)
        if (isTutorialActive)
        {
            if (Input.anyKeyDown) 
            {
                NextPage(); 
            }
            return; 
        }

        // 2. ЛОГИКА СТАРТА ИГРЫ
        // Добавляем проверку, не проходили ли мы управление раньше
        if (!hasLaunchedGame && Input.GetKeyDown(KeyCode.Space))
        {
            hasLaunchedGame = true;
            StartSequence("Controls"); 
        }
    }

    // --- ЛОГИКА ЗАПУСКА С ПРОВЕРКОЙ ПАМЯТИ ---

    public void StartSequence(string id)
    {
        // 1. ПРОВЕРКА: Было ли это обучение уже пройдено?
        // Мы используем ключ "Tut_ИМЯ", например "Tut_Intro"
        // Если значение 1 - значит проходили.
        if (PlayerPrefs.GetInt("Tut_" + id, 0) == 1)
        {
            return; // ВЫХОДИМ, ничего не показываем
        }

        TutorialSequence selectedSeq = null;
        foreach (var seq in allSequences)
        {
            if (seq.sequenceID == id)
            {
                selectedSeq = seq;
                break;
            }
        }

        if (selectedSeq == null) return; 

        // 2. СОХРАНЕНИЕ: Сразу помечаем как пройденное
        PlayerPrefs.SetInt("Tut_" + id, 1);
        PlayerPrefs.Save();

        // Дальше старая логика показа...
        selectedSeq.isCompleted = true;
        isTutorialActive = true;
        Time.timeScale = 0f; 
        tutorialPanel.SetActive(true);
        
        if(pressKeyText != null) pressKeyText.gameObject.SetActive(true);

        currentPagesQueue.Clear();
        foreach (var page in selectedSeq.pages)
        {
            currentPagesQueue.Enqueue(page);
        }

        NextPage(); 
    }

    void NextPage()
    {
        if (currentPagesQueue.Count == 0)
        {
            EndTutorial();
            return;
        }

        TutorialPage page = currentPagesQueue.Dequeue();
        ShowPageContent(page);
    }

    void ShowPageContent(TutorialPage page)
    {
        if (tutorialText != null) tutorialText.text = page.text;

        if (page.screenshot != null)
        {
            tutorialImage.gameObject.SetActive(true);
            tutorialImage.sprite = page.screenshot;
        }
        else
        {
            tutorialImage.gameObject.SetActive(false);
        }

        arrowRect.gameObject.SetActive(false); 

        if (page.uiTarget != null)
        {
            PositionArrow(page.uiTarget.position);
        }
        else if (page.worldTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(page.worldTarget.position);
            PositionArrow(screenPos);
        }
    }

    void PositionArrow(Vector3 targetScreenPos)
    {
        arrowRect.gameObject.SetActive(true);
        Vector3 finalPos = targetScreenPos;
        finalPos.x -= arrowOffset; 
        arrowRect.position = finalPos;
        arrowRect.rotation = Quaternion.identity;
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        arrowRect.gameObject.SetActive(false);
        isTutorialActive = false;
        Time.timeScale = 1f; 
    }

    // --- ПОЛЕЗНАЯ ФУНКЦИЯ ДЛЯ ТЕСТОВ ---
    // Чтобы ты мог сам сбросить обучение и проверить всё заново.
    // Нажми правой кнопкой на скрипт в инспекторе -> Reset Tutorials
    [ContextMenu("Reset Tutorials")] 
    public void ResetTutorialMemory()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Память обучения очищена! При следующем запуске туторы появятся снова.");
    }
}