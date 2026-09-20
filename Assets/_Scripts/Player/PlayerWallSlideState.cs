using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public float wallSlideSpeed = 2f;

    public PlayerWallSlideState(Player player) : base(player){}

    public override void Enter()
    {
        base.Enter();      
        anim.SetBool("isWallSliding", true);

        rb.linearVelocity = new Vector2(0, 0);
    }

    public override void Update()
    {
        base.Update();
        if (JumpPressed){
            JumpPressed = false;
            player.ChangeState(player.wallJumpState);
        }
        else if (!player.isTouchingWall || Mathf.Abs(MoveInput.x) < .1f)
        {
            player.ChangeState(player.fallState);
        }
        else if (player.isGrounded)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        rb.linearVelocity = new Vector2(0, -wallSlideSpeed);
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("isWallSliding", false);
    }
}
