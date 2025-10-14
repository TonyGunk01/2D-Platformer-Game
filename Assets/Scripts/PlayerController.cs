using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public ScoreController scoreController;
    public GameOverController gameOverController;

    public float speed;
    public float jump;

    private Rigidbody2D rb2d;

    private void Awake()
    {
        animator.SetBool("Dead", false);
        Debug.Log("Player Controller Awake");
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    public void PickUpKey()
    {
        Debug.Log("Player picked up the key!");
        animator.SetTrigger("PickUpKey");
        scoreController.AddScore(10);
    }

    // kill player and play death animation

    public void KillPlayer()
    {
        Debug.Log("Player died!");
        animator.SetBool("Dead", true); 

        gameOverController.PlayerDied();
        StartCoroutine(Delay(1f));

        this.enabled = false; // disable player controller
        gameOverController.Awake();
    }

    //delay respawn to allow death animation to play

    public IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
            Debug.Log("Collision: " + collision.gameObject.name);
    }*/

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Jump");

        MoveCharacter(horizontal, vertical);
        PlayMovementAnimation(horizontal, vertical);
    }

    private void MoveCharacter(float horizontal, float vertical)
    {
        // horizontal movement
        Vector3 position = transform.position;
        position.x += horizontal * speed * Time.deltaTime;
        transform.position = position;

        // vertical movement
        if (vertical > 0)
        {
            rb2d.AddForce(new Vector2(0f, jump), ForceMode2D.Force);
        }
    }

    private void PlayMovementAnimation(float horizontal, float vertical)
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        Vector3 scale = transform.localScale;

        if (horizontal < 0)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }

        else if (horizontal > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;

        // jump
        
        if (vertical > 0)
        {             
            animator.SetBool("Jump", true);
        }

        else
        {
            animator.SetBool("Jump", false);
        }
    }
}