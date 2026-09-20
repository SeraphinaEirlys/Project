using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private bool hasLanded;

    public PlayerJumpState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        anim.SetBool("isJumping", true);
        anim.SetBool("isJumpCut", false);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);

        JumpPressed = false;
        JumpReleased = false;
        hasLanded = false;
    }

    public override void Update()
    {
        base.Update();

        if(!player.isGrounded && player.isTouchingWall && MoveInput.x == player.facingDirection && rb.linearVelocity.y < 0)
        {
            player.ChangeState(player.wallSlideState);
        }

        else if(JumpPressed && player.isTouchingWall)
        {
            player.ChangeState(player.wallJumpState);
        }

        else if (rb.linearVelocity.y < -.1f && !player.isGrounded)
        {
            player.ChangeState(player.fallState);
            return;
        }

        if (!hasLanded && player.isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            hasLanded = true;
            anim.SetBool("isJumpCut", false);
            player.attackBuffered = false;
            
            if (ShouldPlayLandAnimation())
                player.ChangeState(player.landState);
            else
                player.ChangeState(player.idleState);

            return;
        }

        if (player.ConsumeAirAttackBuffer() && player.CanAirAttack)
        {
            player.ChangeState(player.airAttackState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.ApplyVariableGravity();

        if (JumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * player.jumpCutMultiplier);
            JumpReleased = false;
            anim.SetBool("isJumpCut", true);
        }

        float speed = RunPressed ? player.runSpeed : player.walkSpeed;
        float targetSpeed = speed * MoveInput.x;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private bool ShouldPlayLandAnimation()
    {
        return player.runPressed && Mathf.Abs(MoveInput.x) > 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("isJumping", false);
        anim.SetBool("isJumpCut", false);
    }
}