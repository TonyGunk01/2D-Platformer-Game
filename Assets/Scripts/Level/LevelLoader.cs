using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelLoader : MonoBehaviour
{
    private Button button;
    public string LevelName;

    // optional UI elements to show locked/completed visuals (assign in inspector)
    public GameObject LockedIcon;
    public GameObject CompletedIcon;

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
        if (button == null) button = GetComponent<Button>();

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

        Debug.Log($"LevelLoader: {LevelName} status={status} (interactable={button.interactable}) for user {LevelManager.Instance.CurrentUserId}");
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
