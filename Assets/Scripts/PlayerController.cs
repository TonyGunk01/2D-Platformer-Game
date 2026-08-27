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

    public void KillPlayer()
    {
        if (isDead) 
            return;

        displayText.text = "<color=red>Game Over</color>";
        isDead = true;

        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        if (animator != null)
            animator.SetBool("Dead", true);

        if (rb2d != null)
            rb2d.simulated = false;

        yield return new WaitForSeconds(1.5f);

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