using UnityEngine;

public class DamagedState : State
{
    protected override string AnimBoolName => "isDamaged";

    private Vector2 knockbackVelocity;
    private float knockbackTimer;
    private float stunTimer;

    // knockbackDuration: thời gian đẩy lùi (thường ngắn: 0.15s - 0.2s)
    // stunDuration: tổng thời gian bị khống chế/đơ đòn
    public DamagedState(Enemy enemy, Vector2 knockbackVelocity, float knockbackDuration, float stunDuration) : base(enemy)
    {
        this.knockbackVelocity = knockbackVelocity;
        this.knockbackTimer = knockbackDuration;
        this.stunTimer = Mathf.Max(stunDuration, knockbackDuration); // Đảm bảo stun không ngắn hơn knockback
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.CanAct = false;
        anim.SetBool("isAttacking", false);

        // Gán thẳng vận tốc tức thì -> knockback bén, không bị ì ạch bởi Mass
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

        // 2. Quản lý thời gian Stun
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