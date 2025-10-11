using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BoxCollider2D boxCol;

    private void Awake()
    {
        Debug.Log("Player Controller Awake");
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
            Debug.Log("Collision: " + collision.gameObject.name);
    }*/

    public void Update()
    {
        float speed = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(speed));

        Vector3 scale = transform.localScale;

        if(speed < 0)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }

        else if(speed > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;

        float VerticalInput = Input.GetAxisRaw("Vertical");

        PlayJumpAnimation(VerticalInput);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch(true);
        }

        else
        {
            Crouch(false);
        }
    }

    public void Crouch(bool crouch)
    {
        if (crouch == true)
        {
            float offX = -0.12f;     //Offset X
            float offY = 0.589f;      //Offset Y

            float sizeX = 0.929f;     //Size X
            float sizeY = 1.31f;     //Size Y

            boxCol.size = new Vector2(sizeX, sizeY);   //Setting the size of collider
            boxCol.offset = new Vector2(offX, offY);   //Setting the offset of collider
        }

        else
        {
            //Reset collider to initial values
            boxCol.size = new Vector2(0.0306f, 0.985f);
            boxCol.offset = new Vector2(0.623f, 2.102f);
        }

        //Play Crouch animation
        playerAnimator.SetBool("Crouch", crouch);
    }

    public void PlayJumpAnimation(float vertical)
    {
        if (vertical > 0)
        {
            playerAnimator.SetTrigger("Jump");
        }
    }
}
