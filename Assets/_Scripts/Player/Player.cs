using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerState currentState;

    public PlayerIdleState idleState;
    public PlayerJumpState jumpState;
    public PlayerMoveState moveState;
    public PlayerLandState landState;
    public PlayerCrouchState crouchState;
    public PlayerSlideState slideState;
    public PlayerAttackState attackState;
    public PlayerAirAttackState airAttackState;
    public PlayerFallState fallState;
    public PlayerSpellcastState spellcastState;
    public PlayerWallJumpState wallJumpState;
    public PlayerWallSlideState wallSlideState;
    public PlayerDamagedState damagedState;

    [Header("Core Components")]
    public Combat combat;
    public Damage damage;
    public Magic magic;
    public Health health;

    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput playerInput;
    public Animator anim;
    public CapsuleCollider2D playerCollider;

    [Header("Movement Variables")]
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public float jumpForce;
    public float jumpCutMultiplier = .3f;
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;

    public int facingDirection = 1;

    // Inputs
    public Vector2 moveInput;
    public bool runPressed;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool attackPressed;
    public bool attackBuffered;
    public bool spellcastPressed;

    [Header("Slide Settings")]
    public float slideDuration = .6f;
    public float slideSpeed = 12;
    public float slideStopDuration = .15f;
    public bool slideTriggered;

    [Header("Slide Collider Settings")]
    public Vector2 normalSize = new Vector2(1.3f, 1.9f);
    public Vector2 normalOffset = new Vector2(0f, 0f);
    public Vector2 slideSize = new Vector2(1.3f, 0.5f);

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Crouch Check")]
    public Transform headCheck;
    public float headCheckRadius = .2f;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckRadius = .15f;
    public LayerMask wallLayer;
    public bool isTouchingWall;

    [Header("Attack Cooldown")]
    public float attackCooldown = 0.3f;
    [HideInInspector] public float attackCooldownTimer;

    [Header("Combo Memory")]
    [Tooltip("Đòn vừa đánh xong (0 = không có combo chờ)")]
    public int comboMemoryStep = 0;
    public bool comboMemoryIsRising = false;

    [Tooltip("Thời gian sống của combo memory sau khi clip kết thúc (giây)")]
    public float comboWindowDuration = 1f;
    [HideInInspector] public float comboWindowExpireTime = 0f;

    [Tooltip("Cooldown phạt khi để combo memory hết hạn")]
    public float missedComboCooldown = 0.5f;

    [Header("Air Attack Cooldown")]
    public float airAttackCooldown = 0.5f;
    [HideInInspector] public float airAttackCooldownTimer;

    [HideInInspector] public bool isFacingLocked;

    private Vector3 originalSpritePosition;

    public bool CanAirAttack => airAttackCooldownTimer <= 0f;
    public bool CanAttack => attackCooldownTimer <= 0f;
    public bool HasComboMemory => comboMemoryStep > 0 && Time.time <= comboWindowExpireTime;

    public void WallJumpLaunch() => currentState.WallJumpLaunch();
    public void LockFacing() => isFacingLocked = true;
    public void UnlockFacing() => isFacingLocked = false;

    private void Awake()
    {
        if (playerCollider != null)
        {
            normalSize = playerCollider.size;
            normalOffset = playerCollider.offset;
        }

        idleState = new PlayerIdleState(this);
        jumpState = new PlayerJumpState(this);
        moveState = new PlayerMoveState(this);
        landState = new PlayerLandState(this);
        crouchState = new PlayerCrouchState(this);
        slideState = new PlayerSlideState(this);
        attackState = new PlayerAttackState(this);
        airAttackState = new PlayerAirAttackState(this);
        fallState = new PlayerFallState(this);
        spellcastState = new PlayerSpellcastState(this);
        wallJumpState = new PlayerWallJumpState(this);
        wallSlideState = new PlayerWallSlideState(this);
        damagedState = new PlayerDamagedState(this);
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;
        ChangeState(idleState);

        originalSpritePosition = transform.position;
    }

    void Update()
    {
        if (attackCooldownTimer > 0f) attackCooldownTimer -= Time.deltaTime;
        if (airAttackCooldownTimer > 0f) airAttackCooldownTimer -= Time.deltaTime;

        if (comboMemoryStep > 0 && Time.time > comboWindowExpireTime)
        {
            ResetComboMemory();
            attackCooldownTimer = Mathf.Max(attackCooldownTimer, missedComboCooldown);
        }

        if (attackBuffered && CanAttack && HasComboMemory && currentState != attackState && currentState != airAttackState && isGrounded)
        {
            ChangeState(attackState);
        }

        Flip();
        currentState.Update();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
        CheckGrounded();
        CheckForWalls();
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void SetColliderNormal()
    {
        playerCollider.direction = CapsuleDirection2D.Vertical;
        playerCollider.size = normalSize;
        playerCollider.offset = normalOffset;
    }

    public void SetColliderSlide()
    {
        playerCollider.direction = CapsuleDirection2D.Horizontal;
        playerCollider.size = slideSize;

        float bottomY = normalOffset.y - (normalSize.y / 2f);
        float newOffsetY = bottomY + (slideSize.y / 2f);

        playerCollider.offset = new Vector2(normalOffset.x, newOffsetY);
    }

    public void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y < -.1f)
            rb.gravityScale = fallGravity;
        else if (rb.linearVelocity.y > .1f)
            rb.gravityScale = jumpGravity;
        else
            rb.gravityScale = normalGravity;
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void CheckForWalls()
    {
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }

    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
    }

    void HandleAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void Flip()
    {
        if (isFacingLocked) return;

        if (moveInput.x > .1f)
            facingDirection = 1;
        else if (moveInput.x < -.1f)
            facingDirection = -1;

        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    public void AttackAnimationFinished() => currentState.AttackAnimationFinish();
    public void AnimationFinished() => currentState.AnimationFinished();

    public void OnMove(InputValue value)
    {
        Vector2 lastMoveInput = moveInput;
        moveInput = value.Get<Vector2>();

        if (isGrounded && lastMoveInput.y > -0.1f && moveInput.y <= -0.1f && runPressed)
            slideTriggered = true;
    }

    public void OnRun(InputValue value)
    {
        bool wasPressed = runPressed;
        runPressed = value.isPressed;
        if (isGrounded && !wasPressed && runPressed && moveInput.y <= -0.1f)
            slideTriggered = true;
    }

    public void OnAttack(InputValue value)
    {
        attackPressed = value.isPressed;
        if (value.isPressed) attackBuffered = true;
    }

    private void OnLeftShoulder(InputValue value)
    {
        if (value.isPressed) magic.PreviousSpell();
    }

    private void OnRightShoulder(InputValue value)
    {
        if (value.isPressed) magic.NextSpell();
    }

    public void OnSpellcast(InputValue value)
    {
        spellcastPressed = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isGrounded && !CheckForCeiling() || isTouchingWall)
                jumpPressed = true;

            jumpReleased = false;
        }
        else
        {
            jumpPressed = false;
            jumpReleased = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }

    public void FinishLandingAnimation()
    {
        if (currentState is PlayerLandState landState)
            landState.isInputLocked = false;
    }

    public bool ConsumeSlideTrigger()
    {
        if (slideTriggered)
        {
            slideTriggered = false;
            return true;
        }
        return false;
    }

    public bool ConsumeAttackBuffer()
    {
        if (attackBuffered && CanAttack)
        {
            attackBuffered = false;
            return true;
        }
        return false;
    }

    public bool ConsumeAirAttackBuffer()
    {
        if (attackBuffered && CanAirAttack)
        {
            attackBuffered = false;
            return true;
        }
        return false;
    }

    public void TriggerAttackCooldown()
    {
        attackCooldownTimer = attackCooldown;
        attackBuffered = false;
    }

    public void TriggerAirAttackCooldown()
    {
        airAttackCooldownTimer = airAttackCooldown;
        attackBuffered = false;
    }

    public void ResetVisualPosition()
    {
        transform.position = new Vector3(transform.position.x, originalSpritePosition.y, transform.position.z);
    }

    public void SaveComboMemory(int currentStep, bool isRising, float duration)
    {
        comboMemoryStep = currentStep;
        comboMemoryIsRising = isRising;
        comboWindowExpireTime = Time.time + duration;
    }

    public void ResetComboMemory()
    {
        comboMemoryStep = 0;
        comboMemoryIsRising = false;
        comboWindowExpireTime = 0f;
    }

    public bool CanWallSlide => !isGrounded && isTouchingWall && (moveInput.x * facingDirection > 0.1f);
}