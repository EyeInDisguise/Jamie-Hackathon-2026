using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Stuff")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;
    
    [Header("Jump Stuff")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 2f;
    
    [Header("Ac/De-eleration")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;
    
    [Header("FallSpeed")]
    [SerializeField] private float maxFallSpeed = 20f;
    
    [Header("Dashing Stuff")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing;
    private bool canDash = true;

    
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Input System
    private PlayerControls controls;

    // Movement state
    private float horizontalInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float velocityXSmoothing;

    // Jump 
    private bool jumpInputHeld;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;
    
    // Stores current ability
    private int currentAbility = 0;
    
    private void Awake()
    {
        // Initialises all the components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        controls = new PlayerControls();
    }
    
    private void OnEnable()
    {
        controls.Player.Enable();
        // Jump
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        // Jump
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;
        controls.Player.Disable();
    }

   private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
    jumpBufferCounter = jumpBufferTime;
    }   
   
    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if (rb.linearVelocity.y > 0f) // off ground and going up instead of when going down.
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f); // 0.5f is for grav
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {
        horizontalInput = controls.Player.Move.ReadValue<float>();
        
        CheckGroundStatus();
        //Debug
        //Debug.Log(isGrounded);
        
        // To check if player just landed on the ground (animation)
        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
        }
        
        // Reset canDash
        if (isGrounded)
        {
            canDash = true;
        }
        
        // Detect when landed
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            isJumping = false;
        }

        // For the coyote jump timer
        if (isGrounded && !isJumping)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        // Timer for jump buffer
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        // Jump
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            isJumping = true;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
        
        // Flip the player depending on direction
        if (horizontalInput > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            spriteRenderer.flipX = true;
        }
        
        // For animation logic
        // For the speed, acceleration is also accounted for instead of just input cause moving direction will make it 0
        float animationSpeed = Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(rb.linearVelocity.x) / moveSpeed);
        animator.SetFloat("Speed", animationSpeed);   
        
        animator.SetBool("Jumping", isJumping);
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
        animator.SetBool("Grounded", isGrounded);
        
        // Abilities
        if (Keyboard.current.digit1Key.wasPressedThisFrame) currentAbility = 1;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) currentAbility = 2;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) currentAbility = 3;
        if (Keyboard.current.digit4Key.wasPressedThisFrame) currentAbility = 4;
        Debug.Log("Ability: " + currentAbility);
        
        // Dashing
        // If 1 (dash) is stored and dash is pressed (shift), then do dash logic
        if (currentAbility == 1 && Keyboard.current.leftShiftKey.wasPressedThisFrame && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        // Stops from overriding the dash
        if (isDashing) return;
        
        // For acceleration/deceleration 
        float targetSpeed = horizontalInput * moveSpeed;

        float rate = horizontalInput != 0 ? acceleration : deceleration;
        
        // Instead of straight to 0->8, goes like 0->1->2....->8
        float newXVelocity = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * 
            Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
        
        // When going down, adds faster falling gravity
        if (rb.linearVelocity.y < 0f) 
        { 
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallGravityMultiplier - 1f) * 
                                               Time.fixedDeltaTime);
        }
        // Max fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }
    
    private void CheckGroundStatus()
    {
        wasGrounded = isGrounded;

        // Just checks if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,groundLayer);
    }

    private IEnumerator Dash()
    {
        // Initialise 
        canDash = false;
        isDashing = true;
        // set gravity to 0
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        // actual dash logic 
        // Calculate direction
        Vector2 dashDirection = controls.Player.DashDirection.ReadValue<Vector2>();

        if (dashDirection == Vector2.zero)
        {
            dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }
        // Normalize makes diagonal faster cause like a triangle (side is longer on angle), makes it all same
        dashDirection.Normalize();
        
        // Actual dash movement
        rb.linearVelocity = dashDirection * dashSpeed;
    
        // Sideway dash for animator logic (don't play animation if dashing up or down)
        bool sidewaysDash = Mathf.Abs(dashDirection.x) > 0.1f;

        // For the animations, dashing upwards plays jump rise, dash down plays jump fall, else play dash
        if (sidewaysDash)
        {
            animator.Play("Dash", 0, 0f);
        }
        else if (dashDirection.y > 0f)
        {
            animator.Play("JumpRise", 0, 0f);
        }
        else if (dashDirection.y < 0f)
        {
            animator.Play("JumpFall", 0, 0f);
        }
        
        // resets gravity to normal
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        // For animator
        animator.SetBool("Dashing", false);
    }
}       
