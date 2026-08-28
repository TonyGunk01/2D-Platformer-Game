using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOverController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            // Save stats when level is completed
            int coins = 0;
            float time = 0f;

            if (player.scoreController != null)
                coins = player.scoreController.GetScore();

            if (player.uiTimer != null)
                time = player.uiTimer.GetCurrentTime();

            StatsManager.SaveStats(coins, time);

            LevelManager.Instance.MarkCurrentLevelComplete();
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex <= 4)
                SceneManager.LoadScene(currentSceneIndex+1);
        }
    }
}