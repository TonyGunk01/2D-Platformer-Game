using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelLoader : MonoBehaviour
{
    private Button button;
    public string LevelName;

    public GameObject LockedIcon;
    public GameObject CompletedIcon;
    public TMP_Text ScoreText;
    public TMP_Text TimeText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    private void OnEnable()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (button == null) 
            button = GetComponent<Button>();

        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("LevelLoader: LevelManager instance not found. Disabling level button: " + LevelName);
            button.interactable = false;
            return;
        }

        LevelStatus status = LevelManager.Instance.GetLevelStatus(LevelName);
        button.interactable = (status != LevelStatus.Locked);

        if (LockedIcon != null)
            LockedIcon.SetActive(status == LevelStatus.Locked);

        if (CompletedIcon != null)
            CompletedIcon.SetActive(status == LevelStatus.Completed);

        if (ScoreText != null)
        {
            int score = LevelManager.Instance.GetLevelScore(LevelName);
            ScoreText.text = score >= 0 ? score.ToString() : "-";
        }

        if (TimeText != null)
        {
            float time = LevelManager.Instance.GetLevelTime(LevelName);
            TimeText.text = time >= 0f ? FormatTime(time) : "-";
        }
    }

    private string FormatTime(float seconds)
    {
        int totalMilliseconds = Mathf.Max(0, Mathf.RoundToInt(seconds * 1000f));
        int minutes = totalMilliseconds / 60000;
        int secs = (totalMilliseconds % 60000) / 1000;
        int centis = (totalMilliseconds % 1000) / 10;

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, secs, centis);
    }

    private void onClick()
    {
        LevelStatus levelStatus = LevelManager.Instance.GetLevelStatus(LevelName);
        switch (levelStatus)
        {
            case LevelStatus.Locked:
                break;

            case LevelStatus.Unlocked:
            case LevelStatus.Completed:
                SoundManager.Instance.Play(Sounds.ButtonClick);
                Time.timeScale = 1f;

                SceneManager.LoadScene(LevelName);
                break;
        }
    }
}
