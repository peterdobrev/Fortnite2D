using Unity.Netcode;
using UnityEngine;

public class PlayerMovementController2D : NetworkBehaviour
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

    private Vector2 lastServerPosition;

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
        if (IsOwner)
        {
            float moveDirection = GetHorizontalInput();
            bool isJumpPressed = IsJumpInput();
            HandleSprinting();

            HandleLocalMovement(moveDirection, isJumpPressed, isSprinting);

            SendInputToServerRpc(moveDirection, isJumpPressed, isSprinting);
        }

        else
        {
            // For non-owner clients, we might want to use server-provided positions
            // This is a simple approximation and might be improved with interpolation
            transform.position = Vector2.Lerp(transform.position, lastServerPosition, 0.1f);
        }

        HandleJumpAnimation();
        HandleCharacterFlip();
    }

    private void HandleLocalMovement(float moveDirection, bool isJumpPressed, bool isSprinting)
    {
        // Your movement logic, similar to HandleMovement but without any authoritative checks.
        float moveSpeedModified = GetMoveSpeed() * (isSprinting ? sprintSpeedMultiplier : 1f);
        rb.velocity = new Vector2(moveDirection * moveSpeedModified, rb.velocity.y);

        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        // Other movement logic...
    }

    private void HandleMovement(float moveDirection, bool isJumpPressed, bool isSprinting)
    {
        // Your movement logic, similar to HandleMovement but without any authoritative checks.
        float moveSpeedModified = GetMoveSpeed() * (isSprinting ? sprintSpeedMultiplier : 1f);
        rb.velocity = new Vector2(moveDirection * moveSpeedModified, rb.velocity.y);

        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // Other movement logic...
    }

    [ServerRpc]
    private void SendInputToServerRpc(float moveDirection, bool isJumpPressed, bool isSprinting)
    {
        HandleMovement(moveDirection, isJumpPressed, isSprinting);

        // Now send the true position back to the client
        SendPositionToClientRpc(NetworkObject.OwnerClientId, transform.position);
    }

    [ClientRpc]
    private void SendPositionToClientRpc(ulong clientId, Vector2 serverPosition)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            lastServerPosition = serverPosition;
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
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            // Flip the character to the right
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
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
