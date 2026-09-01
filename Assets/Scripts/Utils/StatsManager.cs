using TMPro;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class StatsManager
{
    private const string LastScoreKey = "LastScore";
    private const string LastTimeKey = "LastTime";
    public static event Action<int, float> OnStatsSaved;

    public static void SaveStats(int score, float time)
    {
        PlayerPrefs.SetInt(LastScoreKey, score);
        PlayerPrefs.SetFloat(LastTimeKey, time);
        PlayerPrefs.Save();
        OnStatsSaved?.Invoke(score, time);
    }

    public static void DisplayLastStats(TMP_Text timerText, TMP_Text scoreText)
    {
        int lastScore = GetLastScore();
        float lastTime = GetLastTime();

        if (timerText != null)
            timerText.text = FormatTime(lastTime);

        if (scoreText != null)
            scoreText.text = "Score: " + lastScore;
    }

    public static string FormatTime(float timeSeconds)
    {
        var totalMilliseconds = Mathf.Max(0, Mathf.RoundToInt(timeSeconds * 1000f));
        var minutes = totalMilliseconds / 60000;
        var seconds = (totalMilliseconds % 60000) / 1000;
        var milliseconds = (totalMilliseconds % 1000) / 10;

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public static int GetLastScore()
    {
        return PlayerPrefs.GetInt(LastScoreKey, 0);
    }

    public static float GetLastTime()
    {
        return PlayerPrefs.GetFloat(LastTimeKey, 0f);
    }
}