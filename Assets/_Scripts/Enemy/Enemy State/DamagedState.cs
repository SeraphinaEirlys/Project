using UnityEngine;

public class DamagedState : State
{
    protected override string AnimBoolName => "isDamaged";
    private float knockbackVelocity;
    private float knockbackDuration;
    
    public DamagedState(Enemy enemy, int knockbackDir) : base(enemy)
    {
        knockbackVelocity = knockbackDir * config.knockbackForce;
    }

    public override void Enter()
    {
        base.Enter();

        knockbackDuration = config.knockbackDuration;
        rb.linearVelocity = new Vector2(knockbackVelocity, rb.linearVelocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        knockbackDuration -= Time.fixedDeltaTime;

        Debug.Log($"Remaining Knockback Time: {knockbackDuration}");

        if(knockbackDuration <= 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            stateMachine.ChangeState(new IdleState(enemy));
        }
    }
}
