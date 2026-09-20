using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        anim.SetTrigger("Fall");
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded)
        {
            player.attackBuffered = false;
            player.ChangeState(player.landState);
            return;
        }

        bool pushingAgainstWall = Mathf.Abs(player.moveInput.x) > 0.1f && (Mathf.Sign(player.moveInput.x) == player.facingDirection);
        if (player.isTouchingWall && pushingAgainstWall && rb.linearVelocity.y < 0)
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

        if (player.ConsumeAirAttackBuffer() && player.CanAirAttack)
        {
            if (player.moveInput.y < -0.1f)
            {
                player.ChangeState(player.airAttackState);
                player.airAttackState.StartDownslamDirectly();
            }
            else
            {
                player.ChangeState(player.airAttackState);
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.ApplyVariableGravity();

        float speed = player.runPressed ? player.runSpeed : player.walkSpeed;
        rb.linearVelocity = new Vector2(player.moveInput.x * speed, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }
}