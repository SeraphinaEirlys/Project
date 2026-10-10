using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    public PlayerDeathState deathState;
    public PlayerDashState dashState;

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
    [HideInInspector] public bool isControlLocked = false;

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
    public float wallCheckDistance = .5f;
    public LayerMask wallLayer;
    public bool isTouchingWall;

    [Header("Attack Cooldown")]
    public float attackCooldown = 0.3f;
    [HideInInspector] public float attackCooldownTimer;

    [Header("Dash Settings")]
    public float dashSpeed = 24f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float dashExitMomentum = 0.5f;

    [HideInInspector] public float dashCooldownTimer;
    [HideInInspector] public bool dashPressed;
    public float dashStopDuration = 0.08f;

    public bool CanDash => dashCooldownTimer <= 0f;

    public int comboMemoryStep = 0;
    public bool comboMemoryIsRising = false;

    public float comboWindowDuration = 1f;
    [HideInInspector] public float comboWindowExpireTime = 0f;

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

    [Header("VFX")]
    public GameObject dashSmokePrefab;

    public GameObject afterImagePrefab;
    public float afterImageCooldown = 0.05f;
    [HideInInspector] public float lastAfterImageTime;

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
        deathState = new PlayerDeathState(this);
        dashState = new PlayerDashState(this);
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;
        ChangeState(idleState);

        originalSpritePosition = transform.position;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            dashPressed = false;
            attackBuffered = false;
            return;
        }

        if (isControlLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            attackBuffered = false;
        }

        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer > 0f) attackCooldownTimer -= Time.deltaTime;
        if (airAttackCooldownTimer > 0f) airAttackCooldownTimer -= Time.deltaTime;

        if (dashPressed && dashCooldownTimer <= 0f && currentState != dashState && currentState != deathState && currentState != damagedState)
        {
            dashPressed = false;
            dashCooldownTimer = dashCooldown;
            ChangeState(dashState);
        }

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
        Vector2 checkDirection = Vector2.right * facingDirection;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, checkDirection, wallCheckDistance, wallLayer);
        isTouchingWall = hit.collider != null;
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
        if (currentState == deathState) return;

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

    /*private void OnLeftShoulder(InputValue value)
    {
        if (value.isPressed) magic.PreviousSpell();
    }

    private void OnRightShoulder(InputValue value)
    {
        if (value.isPressed) magic.NextSpell();
    }*/

    public void OnSpellcast(InputValue value)
    {
        Debug.Log($"[Player] OnSpellcast called! isPressed={value.isPressed}, currentState={currentState?.GetType().Name}");

        spellcastPressed = value.isPressed;

        if (value.isPressed && currentState != spellcastState && currentState != deathState && currentState != damagedState)
        {
            if (magic != null)
            {
                Debug.Log("[Player] Calling magic.TryCastHeal()");
                bool result = magic.TryCastHeal();
                Debug.Log($"[Player] TryCastHeal returned: {result}");
            }
            else
            {
                Debug.LogWarning("[Player] magic is NULL!");
            }
        }
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
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.white;
        if (headCheck != null)
            Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);

        Gizmos.color = Color.green;
        if (wallCheck != null)
        {
            Vector3 checkDirection = Vector3.right * facingDirection;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + checkDirection * wallCheckDistance);
        }
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

    public void OnDash(InputValue value)
    {
        dashPressed = Time.timeScale > 0f && value.isPressed;
    }

    public bool CanWallSlide => !isGrounded && isTouchingWall && (moveInput.x * facingDirection > 0.1f);
}