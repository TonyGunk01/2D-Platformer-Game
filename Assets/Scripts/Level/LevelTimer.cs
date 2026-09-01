using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    private Stopwatch stopwatch = new Stopwatch();

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartTimer();
    }

    public void StartTimer()
    {
        stopwatch.Reset();
        stopwatch.Start();
    }

    public void StopTimer()
    {
        if (stopwatch.IsRunning) 
            stopwatch.Stop();
    }

    public long GetElapsedMilliseconds()
    {
        return stopwatch.ElapsedMilliseconds;
    }

    public string GetElapsedFormatted()
    {
        var ts = stopwatch.Elapsed;
        return string.Format("{0:D2}:{1:D2}.{2:D3}", ts.Minutes, ts.Seconds, ts.Milliseconds);
    }

    public void StopAndSaveCurrentLevelTime()
    {
        StopTimer();

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        long ms = GetElapsedMilliseconds();

        PlayerPrefs.SetString($"LevelTime_{buildIndex}", ms.ToString());
        PlayerPrefs.Save();
    }

    public long GetSavedTimeForLevel(int buildIndex)
    {
        if (!PlayerPrefs.HasKey($"LevelTime_{buildIndex}")) 
            return -1;

        if (long.TryParse(PlayerPrefs.GetString($"LevelTime_{buildIndex}"), out var ms)) 
            return ms;

        return -1;
    }
}