using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    private Animator animator;
    private Vector2 direction = Vector2.right;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponentInParent<PlayerController>(); // Add your player logic as needed
            if (playerController != null && animator != null)
            {
                animator.SetBool("Impact", true);
                Destroy(gameObject, 2f);
            }
        }
        else if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}