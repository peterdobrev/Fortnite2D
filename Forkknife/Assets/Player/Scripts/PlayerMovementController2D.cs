using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementController2D : NetworkBehaviour
{
    #region Movement Settings And Properties

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeedMultiplier = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float groundCheckRadius = 0.2f;

    #endregion

    #region
    private float walkingSfxTimer = 0.5f;
    private float remainingTimer = 0;

    private Vector2 lastSoundPosition;
    private float minDistanceForSound = 1.0f; 

    #endregion

    #region Movement Input Settings For Network

    private struct InputState
    {
        public float moveDirection;
        public bool isJumpPressed;
        public bool isSprinting;
        public uint sequenceNumber; // For tracking which inputs we've processed
    }

    private List<InputState> inputHistory = new List<InputState>();
    private uint inputSequenceNumber = 0; // Keep track of how many inputs we've sent

    #endregion

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
            HandleMovementServerAuth();
        }

        HandleJumpAnimation();
        HandleCharacterFlip();
    }

    private void HandleMovementServerAuth()
    {
        float moveDirection = GetHorizontalInput();
        bool isJumpPressed = IsJumpInput();
        bool isSprinting = IsSprinting();

        InputState newInput = new InputState
        {
            moveDirection = moveDirection,
            isJumpPressed = isJumpPressed,
            isSprinting = isSprinting,
            sequenceNumber = inputSequenceNumber++
        };

        inputHistory.Add(newInput);

        HandleMovement(newInput);
    }


    private void HandleMovement(InputState input)
    {
        float moveSpeedModified = GetMoveSpeed() * (input.isSprinting ? sprintSpeedMultiplier : 1f);
        rb.velocity = new Vector2(input.moveDirection * moveSpeedModified, rb.velocity.y);

        /*// Calculate the distance moved since the last sound was played
      float distanceMoved = Mathf.Abs(rb.position.x - lastSoundPosition.x);

       remainingTimer -= Time.deltaTime;

       if (remainingTimer < 0 && distanceMoved > minDistanceForSound)
       {
           MovementSoundServerRpc();
           remainingTimer = walkingSfxTimer;
           lastSoundPosition = rb.position;  // Update the last sound position
       }*/

        if (input.isJumpPressed && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            SoundManager.instance.PlaySound("Jump");
        }
    }

    [ServerRpc]
    private void MovementSoundServerRpc()
    {
        MovementSoundClientRpc();
    }

    [ClientRpc]
    private void MovementSoundClientRpc()
    {
        SoundManager.instance.PlaySound("Moving");
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

    private bool IsSprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isSprinting", true);
            return true;
        }
        else
        {
            animator.SetBool("isSprinting", false);
            return false;
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

}
