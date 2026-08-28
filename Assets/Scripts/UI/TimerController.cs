using UnityEngine;
using TMPro;

public class UIAdvancedTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float currentTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning && Time.timeScale > 0f)
        {
            currentTime += Time.deltaTime;
            DisplayTime(currentTime);
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        int milliseconds = Mathf.FloorToInt((timeToDisplay % 1) * 100);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}