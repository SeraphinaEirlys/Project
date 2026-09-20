using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    [Header("Launch Force")]
    public float horizontalLaunchForce = 6f;
    public float verticalLaunchForce = 14f;

    [Header("Momentum")]
    public float momentumDuration = 0.18f;

    private bool hasLaunched;
    private float momentumTimer;

    public PlayerWallJumpState(Player player) : base(player) {}

    public override void Enter()
    {
        anim.SetTrigger("WallJumpTrigger");
        anim.SetBool("isWallJumping", true);

        hasLaunched = false;
        momentumTimer = 0f;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        JumpPressed = false;
        JumpReleased = false;
    }

    public override void Update()
    {
        if (!hasLaunched)
            return;

        if (player.isGrounded && rb.linearVelocity.y <= .1f)
        {
            player.ChangeState(player.idleState);
            return;
        }

        bool momentumActive = momentumTimer > 0f;

        if (!momentumActive)
        {
            bool pushingIntoWall = Mathf.Abs(MoveInput.x) > 0.1f && Mathf.Sign(MoveInput.x) == player.facingDirection;

            if (player.isTouchingWall && pushingIntoWall && rb.linearVelocity.y < 0)
            {
                player.ChangeState(player.wallSlideState);
                return;
            }

            if (JumpPressed && player.isTouchingWall)
            {
                JumpPressed = false;
                player.ChangeState(player.wallJumpState);
                return;
            }

            if (rb.linearVelocity.y < -0.1f && !player.isTouchingWall)
            {
                player.ChangeState(player.fallState);
                return;
            }
        }
    }

    public override void FixedUpdate()
    {
        if (!hasLaunched)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        player.ApplyVariableGravity();

        if (momentumTimer > 0f)
        {
            momentumTimer -= Time.fixedDeltaTime;
        }
        else
        {
            float speed = RunPressed ? player.runSpeed : player.walkSpeed;
            rb.linearVelocity = new Vector2(MoveInput.x * speed, rb.linearVelocity.y);
        }

        if (JumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * player.jumpCutMultiplier);
            JumpReleased = false;
        }
    }

    public override void WallJumpLaunch()
    {
        hasLaunched = true;
        momentumTimer = momentumDuration;
        rb.gravityScale = player.jumpGravity;

        rb.linearVelocity = new Vector2(-player.facingDirection * horizontalLaunchForce, verticalLaunchForce);
    }

    public override void Exit()
    {
        anim.SetBool("isWallJumping", false);
        hasLaunched = false;
        momentumTimer = 0f;
    }
}