using UnityEngine;

public static class StatsManager
{
    private const string LastCoinsKey = "LastCoins";
    private const string LastTimeKey = "LastTime";

    public static void SaveStats(int coins, float time)
    {
        PlayerPrefs.SetInt(LastCoinsKey, coins);
        PlayerPrefs.SetFloat(LastTimeKey, time);
        PlayerPrefs.Save();
        Debug.Log($"Stats saved. Coins={coins}, Time={time}");
    }

    public static int GetLastCoins()
    {
        return PlayerPrefs.GetInt(LastCoinsKey, 0);
    }

    public static float GetLastTime()
    {
        return PlayerPrefs.GetFloat(LastTimeKey, 0f);
    }
}