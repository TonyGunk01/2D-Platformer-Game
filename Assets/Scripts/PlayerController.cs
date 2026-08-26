using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public ScoreController scoreController;
    public GameMenuController gameMenuController;
    public float speed;
    public float jump;
    public bool isDead = false;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb2d;
    private bool isGrounded;
    private float previousAnimatorSpeed = 1f;
    public TMP_Text displayText;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    public void PickUpKey()
    {
        animator.SetTrigger("PickUpKey");
        scoreController.AddScore(10);
        SoundManager.Instance.Play(Sounds.KeyCollect);
    }

    // UPDATED: Now starts a coroutine instead of stopping the game instantly
    public void KillPlayer()
    {
        // Safety check to ensure the sequence doesn't run multiple times
        if (isDead) return;

        displayText.text = "Game Over";
        isDead = true;

        // Fire the delayed death routine sequence
        StartCoroutine(DeathSequenceRoutine());
    }

    // NEW: Handles the animation wait window before showing menus
    private IEnumerator DeathSequenceRoutine()
    {
        // 1. Play animation at regular speed
        if (animator != null)
            animator.SetBool("Dead", true);

        // 2. Cut simulation so the character doesn't fall through platforms or slide
        if (rb2d != null)
            rb2d.simulated = false;

        // 3. WAIT: Adjust '1.5f' below to match your death animation clip length exactly
        yield return new WaitForSeconds(1.5f);

        // 4. AFTER THE DELAY: Activate menus and freeze global game time
        if (gameMenuController != null)
        {
            gameMenuController.PlayerDied();
            gameMenuController.gameObject.SetActive(true);
        }

        if (animator != null)
        {
            previousAnimatorSpeed = animator.speed;
            animator.speed = 0f;
        }

        Time.timeScale = 0f;

        // Disable script components so Update loop completely shuts down
        this.enabled = false;
    }

    private void Update()
    {
        if ((GetComponent<PauseController>() != null && GetComponent<PauseController>().isPaused) || isDead)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Jump");

        CheckGrounded();
        MoveCharacter(horizontal, vertical);
        PlayMovementAnimation(horizontal, vertical);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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