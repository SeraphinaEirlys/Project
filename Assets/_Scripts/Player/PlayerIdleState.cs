using UnityEngine;

public class PlayerIdleState : PlayerState
{

    public PlayerIdleState (Player player) : base (player) {}


    public override void Enter()
    {
        anim.SetBool("isIdle", true);
    }

    public override void Update()
    {
        base.Update();

        if (!player.isGrounded && rb.linearVelocity.y < -0.1f)
        {
            player.ChangeState(player.fallState);
            return;
        }

        if (SpellcastPressed && magic.CanCast(magic.CurrentSpell))
        {
            player.ChangeState(player.spellcastState);
        }
        else if (player.ConsumeAttackBuffer() && player.CanAttack)
        {
            player.ChangeState(player.attackState);
        }
        else if (JumpPressed && player.isGrounded)
        {
            JumpPressed = false;
            player.ChangeState(player.jumpState);
        }
        else if (player.ConsumeSlideTrigger())
        {
            player.ChangeState(player.slideState);
        }
        else if(Mathf.Abs(MoveInput.x) > .1f)
        {
            player.ChangeState(player.moveState);
        }
        else if(MoveInput.y < -.1f)
        {
            player.ChangeState(player.crouchState);
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        anim.SetBool("isIdle", false);
    }
}
