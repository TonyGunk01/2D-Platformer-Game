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
                {
                    SoundManager.Instance.Play(Sounds.ChomperDeath);
                }

                else if (gameObject.CompareTag("Gunner"))
                {
                    SoundManager.Instance.Play(Sounds.GunnerDeath);
                }

                Destroy(gameObject, 1.5f);
                playerController.Bounce();
            }
        }

        else if (collision.CompareTag("Player"))
        {
            playerController.KillPlayer();
        }
    }
}