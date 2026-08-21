using UnityEngine;
using UnityEngine.InputSystem;


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
    
    [SerializeField] private float maxFallSpeed = 20f;
    
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

    // Jump state
    private bool jumpInputHeld;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;
    
    private void Awake()
    {
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
    }

    private void FixedUpdate()
    {
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
}       
