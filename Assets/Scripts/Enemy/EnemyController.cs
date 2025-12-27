using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponentInParent<PlayerController>();

        Animator animator = GetComponent<Animator>();

        // If the player's CollisionCheck hits the enemy, kill the enemy and bounce the player
        if (collision.CompareTag("CollisionCheck"))
        {
            if (playerController != null)
            {
                if (animator != null)
                    animator.SetTrigger("Dead");

                if (gameObject.CompareTag("Chomper"))
                {
                    SoundManager.Instance.Play(Sounds.ChomperDeath);
                }

                else if (gameObject.CompareTag("Gunner"))
                {
                    SoundManager.Instance.Play(Sounds.GunnerDeath);
                }

                Destroy(gameObject, 1.5f); // Kill the enemy after enemy death animation
                playerController.Bounce();     // Make the player bounce (implement this in PlayerController if not present)
            }
        }

        // Otherwise, if the player hits the enemy from the side, kill the player
        else if (collision.CompareTag("Player"))
        {
            playerController.KillPlayer();
        }
    }
}