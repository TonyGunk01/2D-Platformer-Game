using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelOverController : MonoBehaviour
{
    public TMP_Text displayText;
    public GameObject nextLevelButton;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            int coins = 0;
            float time = 0f;

            if (player.scoreController != null)
                coins = player.scoreController.GetScore();

            if (player.uiTimer != null)
                time = player.uiTimer.GetCurrentTime();

            // save last-session stats for immediate display
            StatsManager.SaveStats(coins, time);

            // persist per-level best score/time and unlock next level
            LevelManager.Instance.SaveCurrentLevelResult(coins, time);

            displayText.text = "<color=green>Level Complete!</color>";
            nextLevelButton.SetActive(true);

            var pause = player.GetComponent<PauseController>();
            if (pause != null)
            {
                if (!pause.isPaused)
                    pause.TogglePause();

                if (pause.displayText != null)
                    pause.displayText.text = "<color=green>Level Complete!</color>";
            }
        }
    }
}