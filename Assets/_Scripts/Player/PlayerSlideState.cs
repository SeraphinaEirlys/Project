using UnityEngine;

public class PlayerSlideState : PlayerState
{
    private float slideTimer;
    private float slideStopTimer;
    private bool isStopping;

    public PlayerSlideState(Player player) : base(player) {}



    public override void Enter()
    {
        base.Enter();

        slideTimer = player.slideDuration;
        slideStopTimer = 0;
        isStopping = false;

        player.SetColliderSlide();
        anim.SetBool("isSliding", true);
    }


    public override void Update()
    {
        base.Update();

        if (!player.isGrounded && rb.linearVelocity.y < -0.1f)
        {
            player.ChangeState(player.fallState);
            return;
        }

        if(JumpPressed && !player.CheckForCeiling())
        {
            player.ChangeState(player.jumpState);
        }
        else if(slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
        }
        else if(!isStopping)
        {
            if(Mathf.Abs(MoveInput.x) > .1f && !player.CheckForCeiling() && MoveInput.y > -.1f)
            {
                player.ChangeState(player.moveState);
                return;
            }

            isStopping = true;
            slideStopTimer = player.slideStopDuration;
        }
        else
        {
            slideStopTimer -= Time.deltaTime;

            if(Mathf.Abs(MoveInput.x) > .1f && !player.CheckForCeiling() && MoveInput.y > -.1f)
            {
                player.ChangeState(player.moveState);
                return;
            }

            if(slideStopTimer <= 0)
            {
                if(player.CheckForCeiling() || MoveInput.y <= -.1f)
                {
                    player.ChangeState(player.crouchState);
                }
                else
                {
                    player.ChangeState(player.idleState);
                }
            }
        }
    }



    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(slideTimer > 0)
        {
            rb.linearVelocity = new Vector2(player.slideSpeed * player.facingDirection, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }




    public override void Exit()
    {
        base.Exit();

        player.SetColliderNormal();
        anim.SetBool("isSliding", false);
    }

}