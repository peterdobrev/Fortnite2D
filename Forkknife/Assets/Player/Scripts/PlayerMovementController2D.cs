using UnityEngine;

public class PlayerMovementController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSprinting;
    private float groundCheckRadius = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Update()
    {
        float moveDirection = GetHorizontalInput();
        float moveSpeedModified = GetMoveSpeed() * (isSprinting ? sprintSpeedMultiplier : 1f);
        rb.velocity = new Vector2(moveDirection * moveSpeedModified, rb.velocity.y);

        if (IsJumpInput() && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        if (IsSprintInputDown())
        {
            isSprinting = true;
        }
        else if (IsSprintInputUp())
        {
            isSprinting = false;
        }

        HandleInAirAnimation();
        HandleCharacterFlip();
    }

    private void HandleInAirAnimation()
    {
        if(!isGrounded)
        {
            animator.SetBool("In Air", true);
        }
        else
        {
            animator.SetBool("In Air", false);
        }
    }    

    private void HandleCharacterFlip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if the mouse is to the left of the character
        if (mousePos.x < transform.position.x)
        {
            // Flip the character to the left
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Flip the character to the right
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private float GetHorizontalInput()
    {
        var movement = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Movement", Mathf.Abs(movement));
        return movement;

    }

    private float GetMoveSpeed()
    {
        return moveSpeed;
    }

    private bool IsJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    private bool IsSprintInputDown()
    {
        return Input.GetButtonDown("Sprint");
    }

    private bool IsSprintInputUp()
    {
        return Input.GetButtonUp("Sprint");
    }
}
