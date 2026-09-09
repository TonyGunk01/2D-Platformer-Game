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
            // migrate device-wide progress into per-user namespace if needed
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
        LevelStatus levelStatus = (LevelStatus)PlayerPrefs.GetInt(GetKey(level), 0);
        return levelStatus;
    }

    public void SetLevelStatus(string level, LevelStatus levelStatus)
    {
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

    // Debug helper: logs all level statuses for the current user
    public void LogAllLevelStatuses()
    {
        if (Levels == null)
        {
            Debug.Log("LevelManager: Levels array is null.");
            return;
        }

        for (int i = 0; i < Levels.Length; i++)
        {
            string level = Levels[i];
            Debug.Log($"Level status for '{level}' (user '{CurrentUserId}'): {GetLevelStatus(level)}");
        }
    }

    public void ResetProgress()
    {
        if (Levels == null)
            return;

        for (int i = 0; i < Levels.Length; i++)
        {
            PlayerPrefs.DeleteKey(GetKey(Levels[i]));
        }

        if (Levels.Length > 0)
            SetLevelStatus(Levels[0], LevelStatus.Unlocked);
    }

    private void MigrateDeviceProgressIfNeeded()
    {
        if (Levels == null || Levels.Length == 0)
            return;

        // if first level already has per-user key, assume migrated
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

    // Ensure current user has initial progress set (migrate any device keys and unlock first level)
    public void EnsureUserInitialized()
    {
        if (Levels == null || Levels.Length == 0)
            return;

        MigrateDeviceProgressIfNeeded();

        if (GetLevelStatus(Levels[0]) == LevelStatus.Locked)
            SetLevelStatus(Levels[0], LevelStatus.Unlocked);
    }
}