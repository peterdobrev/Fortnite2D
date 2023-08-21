using Photon.Pun;
using UnityEngine;

public class PlayerMovementController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeedMultiplier = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSprinting;
    private float groundCheckRadius = 0.2f;

    private PhotonView view;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Update()
    {
        if (view.IsMine)
        {
            float moveDirection = GetHorizontalInput();
            float moveSpeedModified = GetMoveSpeed() * (isSprinting ? sprintSpeedMultiplier : 1f);
            rb.velocity = new Vector2(moveDirection * moveSpeedModified, rb.velocity.y);

            if (IsJumpInput() && isGrounded)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }

            HandleSprinting();

            HandleJumpAnimation();
            HandleCharacterFlip();
        }
    }

    private void HandleJumpAnimation()
    {
        if(!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }    

    private void HandleSprinting()
    {
        if (IsSprintInputDown())
        {
            isSprinting = true;
            animator.SetBool("isSprinting", true);
        }
        else if (IsSprintInputUp())
        {
            isSprinting = false;
            animator.SetBool("isSprinting", false);
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
