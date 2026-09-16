using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class PauseController : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject gameMenu;
    public GameObject ellen;
    public Animator animator;
    public Rigidbody2D rb2d;
    private float previousAnimatorSpeed = 1f;
    private PlayerController playerController;
    public TMP_Text displayText;
    public GameObject gameStats;
    private Animator ellenAnimator;
    private AnimatorUpdateMode previousEllenUpdateMode;
    private bool ellenWasActiveBeforePause = true;
    private bool ellenExceptionApplied = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();

        playerController = GetComponent<PlayerController>();

        if (ellen != null)
            ellenAnimator = ellen.GetComponent<Animator>();
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

        if (ellen != null)
        {
            ellenWasActiveBeforePause = ellen.activeSelf;

            if (isPaused)
            {
                if (ellenAnimator == null)
                    ellenAnimator = ellen.GetComponent<Animator>();

                bool isException = false;

                if (ellenAnimator != null && ellenAnimator.runtimeAnimatorController != null)
                {
                    string rcName = ellenAnimator.runtimeAnimatorController.name;

                    if (rcName == "Ellen Talking" || rcName == "Ellen Death")
                        isException = true;
                }

                if (isException)
                {
                    previousEllenUpdateMode = ellenAnimator.updateMode;
                    ellenAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    ellenExceptionApplied = true;
                }

                else
                {
                    ellen.SetActive(false);
                    ellenExceptionApplied = false;
                }
            }
            else
            {
                ellen.SetActive(ellenWasActiveBeforePause);

                if (ellenExceptionApplied && ellenAnimator != null)
                {
                    ellenAnimator.updateMode = previousEllenUpdateMode;
                    ellenExceptionApplied = false;
                }
            }
        }

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
        
        if (isPaused && playerController != null)
        {
            int score = 0;
            float time = 0f;

            if (playerController.scoreController != null)
                score = playerController.scoreController.GetScore();

            if (playerController.uiTimer != null)
                time = playerController.uiTimer.GetCurrentTime();

            StatsManager.SaveStats(score, time);
        }

        if (!isPaused && displayText != null)
            displayText.text = string.Empty;
    }
}