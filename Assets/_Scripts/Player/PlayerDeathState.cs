using UnityEngine;

public class PlayerDeathState : PlayerState
{
    private float knockbackVelocity;
    private float knockbackDuration;
    private bool isTimeSlow;
    private float originalGroundCheckRadius;

    public PlayerDeathState(Player player) : base(player) {}

    public void SetParameters(int knockbackDirection, float knockbackForce)
    {
        knockbackVelocity = knockbackDirection * knockbackForce;
    }

    public override void Enter()
    {
        base.Enter();

        Time.timeScale = 0.3f;
        isTimeSlow = true;

        anim.SetBool("isDead", true);

        knockbackDuration = damage.knockbackDuration;
        player.rb.linearVelocity = new Vector2(knockbackVelocity, player.rb.linearVelocity.y);

        originalGroundCheckRadius = player.groundCheckRadius;
        player.groundCheckRadius = 0.2f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (knockbackDuration > 0)
        {
            knockbackDuration -= Time.fixedDeltaTime;

            if (knockbackDuration <= 0)
            {
                if (isTimeSlow)
                {
                    Time.timeScale = 1f;
                    isTimeSlow = false;
                }

                if (player.isGrounded)
                {
                    player.rb.linearVelocity = Vector2.zero;
                }
            }
        }
        else
        {
            if (player.isGrounded)
            {
                player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (isTimeSlow)
        {
            Time.timeScale = 1f;
            isTimeSlow = false;
        }

        player.groundCheckRadius = originalGroundCheckRadius;
        anim.SetBool("isDead", false);
    }
}