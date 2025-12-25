using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneButtonWithRef : MonoBehaviour
{
    [SerializeField] private Button button; // ссылка на кнопку
    [SerializeField] private string sceneName; // имя сцены из Inspector

    private void Awake()
    {
        if (button != null)
        {
            // Подписываемся на событие нажатия кнопки
            button.onClick.AddListener(LoadScene);
            
        }
        else
        {
            Debug.LogError("Button is not assigned!");
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty!");
        }
    }
}
