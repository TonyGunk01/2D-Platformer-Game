using UnityEngine;
using TMPro;

public class UIAdvancedTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float currentTime = 0f;

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            currentTime += Time.deltaTime;
            DisplayTime(currentTime);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        int milliseconds = Mathf.FloorToInt((timeToDisplay % 1) * 100);

        timerText.text = string.Format("Time: {0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}