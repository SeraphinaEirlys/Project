using UnityEngine;

public class IdleState : State
{
    private Transform target;
    protected override string AnimBoolName => "isIdling";

    private float idleTimer;

    public IdleState(Enemy enemy) : base(enemy) 
    {
        this.idleTimer = 0f;
    }

    public IdleState(Enemy enemy, float duration) : base(enemy)
    {
        this.idleTimer = duration;
    }

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        target = senses.GetChaseTarget();
        enemy.CurrentTarget = target;

        if (!target)
        {
            stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        enemy.FaceTarget(target);

        if (idleTimer > 0)
        {
            idleTimer -= Time.fixedDeltaTime;
            return; 
        }

        if (senses.IsInMeleeRange(target) && combat.CanMeleeAttack())
        {
            stateMachine.ChangeState(new MeleeAttackState(enemy));
            return;
        }

        if (senses.IsInShootingRange(target))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (combat.CanRangedAttack())
            {
                stateMachine.ChangeState(new RangedAttackState(enemy));
            }
            return;
        }

        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);

        if (distance <= config.turnThreshold)
        {
            return;
        }

        if (senses.IsHittingWall() || senses.IsAtCliff())
        {
            return;
        }

        stateMachine.ChangeState(new ChaseState(enemy));
    }

    public override void Exit()
    {
        base.Exit();
        rb.linearVelocity = Vector2.zero;
    }
}