using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public ScoreController scoreController;
    public GameMenuController gameMenuController;
    public GameObject gameMenu;

    public float speed;
    public float jump;
    public bool isDead = false;
    public bool isPaused = false;
    public float elapsedTime = 0f;
    private bool isTimerActive = true;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb2d;
    private bool isGrounded;
    private float previousAnimatorSpeed = 1f;

    private void Awake()
    {
        Debug.Log("Player Controller Awake");
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    public void PickUpKey()
    {
        Debug.Log("Player picked up the key!");
        animator.SetTrigger("PickUpKey");
        scoreController.AddScore(10);
        SoundManager.Instance.Play(Sounds.KeyCollect);
    }

    public void KillPlayer()
    {
        Debug.Log("Player died!");

        isDead = true;

        if (animator != null)
            animator.SetBool("Dead", true);
        
        if (gameMenuController != null)
        {
            gameMenuController.PlayerDied();
            gameMenuController.gameObject.SetActive(true);
        }

        else
            Debug.LogWarning("PlayerController.gameMenuController is not assigned in the Inspector.");

        if (rb2d != null)
            rb2d.simulated = false;

        if (animator != null)
        {
            previousAnimatorSpeed = animator.speed;
            animator.speed = 0f;
        }

        Time.timeScale = 0f;

        this.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isDead)
            TogglePause();

        if (isTimerActive)
            elapsedTime += Time.deltaTime;

        if (isPaused)
        {
            isTimerActive = false;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Jump");

        CheckGrounded();
        MoveCharacter(horizontal, vertical);
        PlayMovementAnimation(horizontal, vertical);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        Debug.Log($"TogglePause called. isPaused={isPaused}");

        if (gameMenu != null)
        {
            gameMenu.SetActive(isPaused);
            Debug.Log($"gameMenu set active={isPaused}");
        }

        else
            Debug.LogWarning("PlayerController.gameMenu is not assigned in the Inspector.");

        if (animator != null)
        {
            if (isPaused)
            {
                previousAnimatorSpeed = animator.speed;
                animator.speed = 0f;
            }

            else
            {
                isTimerActive = true;
                animator.speed = previousAnimatorSpeed;
            }
        }

        if (rb2d != null)
            rb2d.simulated = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void StopTimer()
    {
        isTimerActive = false;
        Debug.Log("Level Completed in: " + elapsedTime + " seconds");
    }

    private void MoveCharacter(float horizontal, float vertical)
    {
        Vector3 position = transform.position;
        position.x += horizontal * speed * Time.deltaTime;
        transform.position = position;

        if (vertical > 0 && isGrounded)
            rb2d.AddForce(new Vector2(0f, jump), ForceMode2D.Force);
    }

    private void PlayMovementAnimation(float horizontal, float vertical)
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        Vector3 scale = transform.localScale;

        if (horizontal < 0)
            scale.x = -1f * Mathf.Abs(scale.x);

        else if (horizontal > 0)
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;

        if (vertical > 0)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Grounded", false);
        }

        else
        {
            if (!isDead)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Grounded", isGrounded);
            }
        }
    }

    public void Bounce()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f);
    }
}