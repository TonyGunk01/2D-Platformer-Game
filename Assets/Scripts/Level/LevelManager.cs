using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static LevelManager Instance {  get {  return instance; } }

    public string SampleScene1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(GetLevelStatus(SampleScene1) == LevelStatus.Locked)
        {
            SetLevelStatus(SampleScene1, LevelStatus.Unlocked);
        }
    }

    public void MarkCurrentLevelComplete()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // set level status to completed
        SetLevelStatus(currentScene.name, LevelStatus.Completed);

        // unlock the next level
        int nextSceneIndex = currentScene.buildIndex + 1;
        Scene nextScene = SceneManager.GetSceneByBuildIndex(nextSceneIndex);
        Debug.Log("Next Scene is valid: " + nextScene.IsValid());
        SetLevelStatus(nextScene.name, LevelStatus.Unlocked);
    }

    public LevelStatus GetLevelStatus(string level)
    {
        LevelStatus levelStatus = (LevelStatus)PlayerPrefs.GetInt(level, 0);
        return levelStatus;
    }

    public void SetLevelStatus(string level, LevelStatus levelStatus)
    {
        PlayerPrefs.SetInt(level, (int)levelStatus);
        Debug.Log("Setting Level: " + level + " Status: " + levelStatus);
    }

}
