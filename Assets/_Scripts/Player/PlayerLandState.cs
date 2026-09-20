using UnityEngine;

public class PlayerLandState : PlayerState
{
    public bool isInputLocked;
    private bool isRunningLand;
    private bool hasMoveInput;
    private float landingTimer;

    public PlayerLandState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.3f, 0);

        isRunningLand = player.runPressed && Mathf.Abs(MoveInput.x) > 0.1f;
        
        anim.SetBool("isLanding", true);
        anim.SetBool("isRunningLand", isRunningLand);

        isInputLocked = !isRunningLand;
        hasMoveInput = Mathf.Abs(MoveInput.x) > 0.1f;
        landingTimer = 0f;
        
        if (isRunningLand)
        {
            if (MoveInput.x > 0.1f)
                player.facingDirection = 1;
            else if (MoveInput.x < -0.1f)
                player.facingDirection = -1;
            player.transform.localScale = new Vector3(player.facingDirection, 1, 1);
        }
    }

    public override void Update()
    {
        base.Update();

        landingTimer += Time.deltaTime;

        if (!player.isGrounded)
        {
            if (player.jumpPressed)
            {
                player.jumpPressed = false;
                player.ChangeState(player.jumpState);
            }
            return;
        }

        if (isRunningLand)
        {
            if (player.jumpPressed)
            {
                player.jumpPressed = false;
                player.ChangeState(player.jumpState);
                return;
            }

            if (hasMoveInput && landingTimer > 0.05f)
            {
                float speed = player.runSpeed * 0.7f;
                float targetSpeed = speed * MoveInput.x;
                rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
            }

            return;
        }


        if (isInputLocked)
        {
            player.jumpPressed = false;
            player.jumpReleased = false;
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
        }
        else
        {
            if (player.jumpPressed)
            {
                player.jumpPressed = false;
                player.ChangeState(player.jumpState);
            }
            else if (Mathf.Abs(MoveInput.x) > 0.1f)
            {
                player.ChangeState(player.moveState);
            }
            else
            {
                player.ChangeState(player.idleState);
            }
        }
    }

    public override void AnimationFinished()
    {
        base.AnimationFinished();
        isInputLocked = false;
        
        if (isRunningLand)
        {
            if (Mathf.Abs(MoveInput.x) > 0.1f)
            {
                player.ChangeState(player.moveState);
            }
            else
            {
                player.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("isLanding", false);
        anim.SetBool("isRunningLand", false);
    }
}