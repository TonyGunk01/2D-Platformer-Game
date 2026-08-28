using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class PauseController : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject gameMenu;
    public Animator animator;
    public Rigidbody2D rb2d;
    private float previousAnimatorSpeed = 1f;
    private PlayerController playerController;
    public TMP_Text displayText;

    public GameObject gameStats;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();

        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (playerController == null || !playerController.isDead))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        displayText.text = "<color=blue>Game Paused</color>";

        if (gameMenu != null)
            gameMenu.SetActive(isPaused);

        if (gameStats != null)
            gameStats.SetActive(!isPaused);

        if (animator != null)
        {
            if (isPaused)
            {
                previousAnimatorSpeed = animator.speed;
                animator.speed = 0f;
            }

            else
                animator.speed = previousAnimatorSpeed;
        }

        if (rb2d != null)
            rb2d.simulated = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        // Save stats when game is paused
        if (isPaused && playerController != null)
        {
            int coins = 0;
            float time = 0f;

            if (playerController.scoreController != null)
                coins = playerController.scoreController.GetScore();

            if (playerController.uiTimer != null)
                time = playerController.uiTimer.GetCurrentTime();

            StatsManager.SaveStats(coins, time);
        }

        if (!isPaused && displayText != null)
            displayText.text = string.Empty;
    }
}