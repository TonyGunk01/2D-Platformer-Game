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
        int prevScore = PlayerPrefs.GetInt(scoreKey, -1);

        if (score > prevScore)
            PlayerPrefs.SetInt(scoreKey, score);

        string timeKey = GetKey(level) + "_time";
        float prevTime = PlayerPrefs.GetFloat(timeKey, -1f);

        if (prevTime < 0f || timeSeconds < prevTime)
            PlayerPrefs.SetFloat(timeKey, timeSeconds);

        SetLevelStatus(level, LevelStatus.Completed);
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
}