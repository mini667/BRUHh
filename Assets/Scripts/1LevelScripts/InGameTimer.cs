using UnityEngine;
using TMPro;

public class InGameTimerTMP : MonoBehaviour
{
    public TMP_Text timerText;

    void Update()
    {
        if (LevelTimer.Instance != null)
        {
            float time = LevelTimer.Instance.GetTime();
            timerText.text = time.ToString("F2") + " сек";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
