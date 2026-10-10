using UnityEngine;

public class DamagedState : State
{
    protected override string AnimBoolName => "isDamaged";

    private Vector2 knockbackVelocity;
    private float knockbackTimer;
    private float stunTimer;

    public DamagedState(Enemy enemy, Vector2 knockbackVelocity, float knockbackDuration, float stunDuration) : base(enemy)
    {
        this.knockbackVelocity = knockbackVelocity;
        this.knockbackTimer = knockbackDuration;
        this.stunTimer = Mathf.Max(stunDuration, knockbackDuration);
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.CanAct = false;
        anim.SetBool("isAttacking", false);

        rb.linearVelocity = knockbackVelocity;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        stunTimer -= Time.fixedDeltaTime;
        if (stunTimer <= 0)
        {
            stateMachine.ChangeState(new IdleState(enemy));
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.CanAct = true;
    }
}