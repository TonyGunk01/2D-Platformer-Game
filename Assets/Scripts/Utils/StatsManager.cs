using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StatsManager
{
    private const string LastCoinsKey = "LastCoins";
    private const string LastTimeKey = "LastTime";
    private const string LastTimeDisplayKey = "LastTimeDisplay";

    public static void SaveStats(int coins, float time)
    {
        PlayerPrefs.SetInt(LastCoinsKey, coins);
        PlayerPrefs.SetFloat(LastTimeKey, time);

        string display = FormatTime(time);
        PlayerPrefs.SetString(LastTimeDisplayKey, display);

        PlayerPrefs.Save();
        Debug.Log($"Stats saved. Coins={coins}, Time={display}");

        if (!AuthSessionManager.IsLoggedIn)
            return;

        try
        {
            string username = AuthSessionManager.CurrentUsername;
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string saveDirectory = Path.Combine(projectRoot, "UserDatabase");
            string path = Path.Combine(saveDirectory, username.ToLower().Trim() + ".json");

            if (!File.Exists(path))
            {
                Debug.LogWarning($"User file not found: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            AccountData account = JsonUtility.FromJson<AccountData>(json);
            if (account == null)
            {
                Debug.LogWarning("Failed to parse user account JSON.");
                return;
            }

            string currentLevel = SceneManager.GetActiveScene().name;

            List<AccountData.LevelStats> statsList = new List<AccountData.LevelStats>();
            if (account.levelStats != null)
                statsList.AddRange(account.levelStats);

            AccountData.LevelStats existing = statsList.Find(s => s.levelName == currentLevel);

            bool shouldSave = false;

            if (existing == null)
            {
                existing = new AccountData.LevelStats
                {
                    levelName = currentLevel,
                    bestScore = coins,
                    bestTime = time,
                    bestTimeDisplay = display,
                    date = DateTime.UtcNow.ToString("o")
                };

                statsList.Add(existing);
                shouldSave = true;
            }

            else
            {
                if (coins > existing.bestScore)
                {
                    existing.bestScore = coins;
                    existing.bestTime = time;
                    existing.bestTimeDisplay = display;
                    existing.date = DateTime.UtcNow.ToString("o");
                    shouldSave = true;
                }

                else if (coins == existing.bestScore && time < existing.bestTime)
                {
                    existing.bestTime = time;
                    existing.bestTimeDisplay = display;
                    existing.date = DateTime.UtcNow.ToString("o");
                    shouldSave = true;
                }
            }

            if (shouldSave)
            {
                account.levelStats = statsList.ToArray();
                File.WriteAllText(path, JsonUtility.ToJson(account, true));
                Debug.Log($"Saved best stats for user '{username}' level '{currentLevel}' Score={existing.bestScore} Time={existing.bestTimeDisplay}");
            }

            else
                Debug.Log("New run did not beat personal best - not updating account file.");
        }

        catch (Exception e)
        {
            Debug.LogError($"Error saving account stats: {e.Message}");
        }
    }

    public static int GetLastCoins()
    {
        return PlayerPrefs.GetInt(LastCoinsKey, 0);
    }

    public static float GetLastTime()
    {
        return PlayerPrefs.GetFloat(LastTimeKey, 0f);
    }

    public static string GetLastTimeDisplay()
    {
        return PlayerPrefs.GetString(LastTimeDisplayKey, "00:00:00");
    }

    private static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    [Serializable]
    private class AccountData
    {
        public string username;
        public string passwordHash;
        public string recoveryKey;

        [Serializable]
        public class LevelStats
        {
            public string levelName;
            public int bestScore;
            public float bestTime;
            public string bestTimeDisplay;
            public string date;
        }

        public LevelStats[] levelStats;
    }
}