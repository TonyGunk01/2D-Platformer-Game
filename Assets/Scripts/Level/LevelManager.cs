using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

    public string[] Levels;
    public string[] GloballyUnlockedLevels;
    // Optional: set maximum score (e.g. total coins) and par times for each level in the inspector.
    // Arrays should align with 'Levels' by index.
    public int[] MaxScores;
    public float[] ParTimesSeconds;

    private const string CurrentUserKey = "CurrentUser";

    public string CurrentUserId
    {
        get
        {
            string id = PlayerPrefs.GetString(CurrentUserKey, "guest");
            return string.IsNullOrEmpty(id) ? "guest" : id;
        }
    }

    private string GetKey(string level)
    {
        return $"{CurrentUserId}_{level}";
    }

    private bool IsGloballyUnlocked(string level)
    {
        if (GloballyUnlockedLevels == null)
            return false;

        for (int i = 0; i < GloballyUnlockedLevels.Length; i++)
        {
            if (string.Equals(GloballyUnlockedLevels[i], level, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (Levels != null && Levels.Length > 0)
        {
            MigrateDeviceProgressIfNeeded();

            if (GetLevelStatus(Levels[0]) == LevelStatus.Locked)
                SetLevelStatus(Levels[0], LevelStatus.Unlocked);
        }
    }

    public void MarkCurrentLevelComplete()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SetLevelStatus(currentScene.name, LevelStatus.Completed);

        int currentSceneIndex = Array.FindIndex(Levels, level => level == currentScene.name);
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < Levels.Length)
            SetLevelStatus(Levels[nextSceneIndex], LevelStatus.Unlocked);
    }

    public LevelStatus GetLevelStatus(string level)
    {
        if (IsGloballyUnlocked(level))
            return LevelStatus.Unlocked;

        LevelStatus levelStatus = (LevelStatus)PlayerPrefs.GetInt(GetKey(level), 0);
        return levelStatus;
    }

    public void SetLevelStatus(string level, LevelStatus levelStatus)
    {
        if (IsGloballyUnlocked(level) && levelStatus == LevelStatus.Locked)
            return;

        PlayerPrefs.SetInt(GetKey(level), (int)levelStatus);
        PlayerPrefs.Save();
    }

    public void UnlockLevel(string level)
    {
        SetLevelStatus(level, LevelStatus.Unlocked);
    }

    public void LockLevel(string level)
    {
        SetLevelStatus(level, LevelStatus.Locked);
    }

    public bool IsLevelUnlocked(string level)
    {
        return GetLevelStatus(level) != LevelStatus.Locked;
    }

    public bool IsLevelCompleted(string level)
    {
        return GetLevelStatus(level) == LevelStatus.Completed;
    }

    public void ResetProgress()
    {
        if (Levels == null)
            return;

        for (int i = 0; i < Levels.Length; i++)
            PlayerPrefs.DeleteKey(GetKey(Levels[i]));

        if (Levels.Length > 0)
            SetLevelStatus(Levels[0], LevelStatus.Unlocked);
    }

    private void MigrateDeviceProgressIfNeeded()
    {
        if (Levels == null || Levels.Length == 0)
            return;

        if (PlayerPrefs.HasKey(GetKey(Levels[0])))
            return;

        bool foundDeviceKey = false;

        for (int i = 0; i < Levels.Length; i++)
        {
            string deviceKey = Levels[i];
            if (PlayerPrefs.HasKey(deviceKey))
            {
                int val = PlayerPrefs.GetInt(deviceKey, 0);
                PlayerPrefs.SetInt(GetKey(Levels[i]), val);
                foundDeviceKey = true;
            }
        }

        if (foundDeviceKey)
            PlayerPrefs.Save();
    }

    public void EnsureUserInitialized()
    {
        if (Levels == null || Levels.Length == 0)
            return;

        MigrateDeviceProgressIfNeeded();

        if (GetLevelStatus(Levels[0]) == LevelStatus.Locked)
            SetLevelStatus(Levels[0], LevelStatus.Unlocked);
    }

    public void SetLevelResult(string level, int score, float timeSeconds)
    {
        if (string.IsNullOrEmpty(level))
            return;

        string scoreKey = GetKey(level) + "_score";
        string timeKey = GetKey(level) + "_time";

        int prevScore = PlayerPrefs.GetInt(scoreKey, -1);
        float prevTime = PlayerPrefs.GetFloat(timeKey, -1f);

        bool shouldSave = false;

        if (prevScore < 0)
            shouldSave = true;

        else if (score > prevScore)
            shouldSave = true;

        else if (score == prevScore)
        {
            if (prevTime < 0f || timeSeconds < prevTime)
                shouldSave = true;
        }

        if (shouldSave)
        {
            PlayerPrefs.SetInt(scoreKey, score);
            PlayerPrefs.SetFloat(timeKey, timeSeconds);
        }
        // Mark completed
        SetLevelStatus(level, LevelStatus.Completed);

        // Compute stars according to criteria:
        // 1) completed but not maximum score -> 1 star
        // 2) completed and maximum score -> 2 stars
        // 3) maximum score and completed under certain time (par) -> 3 stars
        int newStars = ComputeStarsForResult(level, score, timeSeconds);
        // Only save if improved
        int prevStars = GetLevelStars(level);
        if (newStars > prevStars)
            SetLevelStars(level, newStars);
        PlayerPrefs.Save();
    }

    public void SaveCurrentLevelResult(int score, float timeSeconds)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SetLevelResult(currentScene.name, score, timeSeconds);

        int currentSceneIndex = Array.FindIndex(Levels, level => level == currentScene.name);
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < Levels.Length)
            SetLevelStatus(Levels[nextSceneIndex], LevelStatus.Unlocked);
    }

    public int GetLevelScore(string level)
    {
        return PlayerPrefs.GetInt(GetKey(level) + "_score", -1);
    }

    public float GetLevelTime(string level)
    {
        return PlayerPrefs.GetFloat(GetKey(level) + "_time", -1f);
    }

    // Stars handling
    private string GetStarsKey(string level)
    {
        return GetKey(level) + "_stars";
    }

    public void SetLevelStars(string level, int stars)
    {
        if (string.IsNullOrEmpty(level))
            return;

        stars = Mathf.Clamp(stars, 0, 3);
        PlayerPrefs.SetInt(GetStarsKey(level), stars);
        PlayerPrefs.Save();
    }

    public int GetLevelStars(string level)
    {
        return PlayerPrefs.GetInt(GetStarsKey(level), 0);
    }

    private int ComputeStarsForResult(string level, int score, float timeSeconds)
    {
        // Default: if not completed or invalid inputs, 0
        int maxScore = GetMaxScoreForLevel(level);
        float par = GetParTimeForLevel(level);
        bool hasMaxScore = maxScore > 0 && score >= maxScore;
        // Debug information to help diagnose star calculation issues
        try
        {
            Debug.Log($"ComputeStarsForResult: level={level}, score={score}, timeSeconds={timeSeconds}, maxScore={maxScore}, par={par}, hasMaxScore={hasMaxScore}");
        }
        catch (Exception ex)
        {
            Debug.Log("ComputeStarsForResult: failed to format debug log: " + ex);
        }

        if (!hasMaxScore && score >= 0)
            return 1; // completed but not max

        if (hasMaxScore)
        {
            if (par > 0f && timeSeconds > 0f && timeSeconds <= par)
                return 3; // max score and under par -> 3 stars

            return 2; // max score but not under par -> 2 stars
        }

        return 0;
    }

    private int GetMaxScoreForLevel(string level)
    {
        if (Levels == null || MaxScores == null)
            return 0;

        for (int i = 0; i < Levels.Length && i < MaxScores.Length; i++)
        {
            if (string.Equals(Levels[i], level, StringComparison.Ordinal))
                return MaxScores[i];
        }

        return 0;
    }

    private float GetParTimeForLevel(string level)
    {
        if (Levels == null || ParTimesSeconds == null)
            return 0f;

        for (int i = 0; i < Levels.Length && i < ParTimesSeconds.Length; i++)
        {
            if (string.Equals(Levels[i], level, StringComparison.Ordinal))
                return ParTimesSeconds[i];
        }

        return 0f;
    }
}