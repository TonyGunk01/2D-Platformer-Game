using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponentInParent<PlayerController>();

        Animator animator = GetComponent<Animator>();

        if (collision.CompareTag("CollisionCheck"))
        {
            if (playerController != null)
            {
                if (animator != null)
                    animator.SetTrigger("Dead");

                if (gameObject.CompareTag("Chomper"))
                    SoundManager.Instance.Play(Sounds.ChomperDeath);

                else if (gameObject.CompareTag("Gunner"))
                    SoundManager.Instance.Play(Sounds.GunnerDeath);

                Collider2D enemyCollider = GetComponent<Collider2D>();
                if (enemyCollider != null)
                    enemyCollider.enabled = false;

                Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
                foreach (Collider2D childCol in childColliders)
                {
                    childCol.enabled = false;
                }

                Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    rb2d.simulated = false;
                    rb2d.linearVelocity = Vector2.zero;
                }

                this.enabled = false;

                Destroy(gameObject, 1.5f);
                playerController.Bounce();
            }
        }

        else if (collision.CompareTag("Player"))
        {
            if (playerController != null && !playerController.isDead)
                playerController.KillPlayer();
        }
    }
}