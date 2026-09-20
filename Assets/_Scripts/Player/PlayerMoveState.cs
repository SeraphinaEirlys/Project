using UnityEngine;

public class PlayerMoveState : PlayerState
{


    public PlayerMoveState(Player player) : base(player){}

    public override void Enter()
    {
        base.Enter();
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
        else if(JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if (player.ConsumeSlideTrigger())
        {
            player.ChangeState(player.slideState);
        }
        else if(Mathf.Abs(MoveInput.x) < .1f)
        {
            player.ChangeState(player.idleState);
        }
        else
        {
            anim.SetBool("isWalking", !RunPressed);
            anim.SetBool("isRunning", RunPressed);
        }
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float speed = RunPressed ? player.runSpeed : player.walkSpeed;
        rb.linearVelocity = new Vector2(speed * player.facingDirection, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
    }
}
