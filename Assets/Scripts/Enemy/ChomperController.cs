using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChomperController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;
    public float sightDistance = 8f;
    public LayerMask obstacleMask;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    public Vector2 groundCheckOffset = new Vector2(0f, -0.1f);

    private Transform targetPoint;
    private Transform playerTransform;
    private Animator animator;

    private void Start()
    {
        targetPoint = pointB;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (pointA == null || pointB == null)
            return;

        bool chasing = playerTransform != null && IsPlayerInLineOfSight();

        if (animator != null)
        {
            animator.SetBool("Chasing", chasing);
            animator.SetBool("Patrolling", !chasing);
        }

        float minX = Mathf.Min(pointA.position.x, pointB.position.x);
        float maxX = Mathf.Max(pointA.position.x, pointB.position.x);

        if (chasing)
        {
            SoundManager.Instance.Play(Sounds.ChomperAttack);
            float playerX = playerTransform.position.x;
            float chomperX = transform.position.x;

            float direction = Mathf.Sign(playerX - chomperX);
            float nextX = chomperX + direction * chaseSpeed * Time.deltaTime;
            Vector2 nextPosition = new Vector2(nextX, transform.position.y);

            if (IsGroundAtEdgeWithPlatformTag(nextPosition))
            {
                Vector2 targetPosition = new Vector2(playerX, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
            }

            Vector3 scale = transform.localScale;

            if (playerX < chomperX)
                scale.x = Mathf.Abs(scale.x);

            else
                scale.x = -Mathf.Abs(scale.x);

            transform.localScale = scale;
        }

        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            Vector3 scale = transform.localScale;

            if (targetPoint.position.x < transform.position.x)
                scale.x = Mathf.Abs(scale.x);

            else
                scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
                targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }

    private bool IsGroundAtEdgeWithPlatformTag(Vector2 checkPosition)
    {
        Vector2 checkPos = checkPosition + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null && hit.collider.gameObject.CompareTag("Platform");
    }

    private bool IsPlayerInLineOfSight()
    {
        float yThreshold = 2f;
        if (Mathf.Abs(playerTransform.position.y - transform.position.y) > yThreshold)
            return false;

        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > sightDistance)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, ~obstacleMask);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            return true;

        return false;
    }
}