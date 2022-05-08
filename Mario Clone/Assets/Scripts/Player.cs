using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpBuffer = 0.25f;
    public float jumpTimer;

    [Header("Components")]
    public Rigidbody2D rb;
    public LayerMask groundLayer;

    [Header("Physics")]
    public float maxSpeed = 8f;
    public float linearDrag = 4f;
    public float gravity = 1;
    public float fallMultiplier = 5f;
    public float vLinearDragMultiplier = 0.55f;

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 1.1f;
    public Vector3 colliderOffset;

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        // Jump buffer so that Clorio jumps after input mid-air after hitting ground again
        if(Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpBuffer;
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        moveCharacter(direction.x);

        if(jumpTimer > Time.time && onGround)
        {
            Jump();
        }

        modifyPhysics();
    }

    // Moves/ Flips/ Sets max speed
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            Flip();
        }
        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    // Horizontal and vertical drag method
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x > 0 && rb.velocity.x < 0);

        if(onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0f;
        }else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * vLinearDragMultiplier;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;

            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    // Flips Clorio on input
    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    // Ground collision detection gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
