using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(Player player) : base(player){}




    public override void Enter()
    {
        base.Enter();

        anim.SetBool("isCrouching", true);
        player.SetColliderSlide();
    }


    public override void Update()
    {
        base.Update();

        if (JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if (player.slideTriggered)
        {
            player.slideTriggered = false;
            player.ChangeState(player.slideState);
        }
        else if(MoveInput.y > -.1f && !player.CheckForCeiling())
        {
            player.ChangeState(player.idleState);
        }
        else if (player.ConsumeSlideTrigger())
        {
            player.ChangeState(player.slideState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }



    public override void Exit()
    {
        base.Exit();

        anim.SetBool("isCrouching", false);
        player.SetColliderNormal();
    }
}