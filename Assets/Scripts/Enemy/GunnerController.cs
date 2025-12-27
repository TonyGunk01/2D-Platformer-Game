using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GunnerController : MonoBehaviour
{
    public float detectionRange = 10f;
    public float yThreshold = 2f;
    public LayerMask obstacleMask;
    public GameObject bulletPrefab; // Assign in Inspector
    public Transform firePoint;     // Assign in Inspector (where bullets spawn)
    public float fireRate = 1f;     // Bullets per second

    private Animator animator;
    private Transform playerTransform;
    private float fireCooldown = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void Update()
    {
        bool playerDetected = playerTransform != null && IsPlayerInLineOfSight();
        if (playerTransform != null)
            FacePlayer();

        if (animator != null)
            animator.SetBool("PlayerDetected", playerDetected);

        if (playerDetected)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                FireBullet();
                SoundManager.Instance.Play(Sounds.BulletFire);
                fireCooldown = 1f / fireRate;
            }
        }
        else
        {
            fireCooldown = 0f;
        }
    }

    private bool IsPlayerInLineOfSight()
    {
        if (Mathf.Abs(playerTransform.position.y - transform.position.y) > yThreshold)
            return false;

        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > detectionRange)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, ~obstacleMask);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            return true;

        return false;
    }

    private void FacePlayer()
    {
        float playerX = playerTransform.position.x;
        float gunnerX = transform.position.x;
        Vector3 scale = transform.localScale;
        // Flip the gunner to face the player if needed
        if ((playerX > gunnerX && scale.x < 0) || (playerX < gunnerX && scale.x > 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null || playerTransform == null)
            return;

        // Calculate horizontal direction only (ignoring vertical)
        float directionX = (playerTransform.position.x > firePoint.position.x) ? 1f : -1f;
        Vector2 direction = new Vector2(directionX, 0f);

        // Rotation: no rotation needed or set to zero, bullets face right by default
        Quaternion rotation = Quaternion.identity;
        if (directionX < 0)
        {
            // Flip rotation for left direction if needed
            rotation = Quaternion.Euler(0, 0, 180f);
        }

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, rotation);
        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if (bullet != null)
            bullet.SetDirection(direction);
    }
}