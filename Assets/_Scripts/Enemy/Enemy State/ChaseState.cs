using UnityEngine;

public class ChaseState : State
{
    private Transform target;
    protected override string AnimBoolName => "isRunning";

    public ChaseState(Enemy enemy) : base(enemy) {}

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        target = senses.GetChaseTarget();
        enemy.CurrentTarget = target;

        if (!target)
        {
            if (config.returnsToStart)
            {
                stateMachine.ChangeState(new ReturnState(enemy));
            }
            else
            {
                stateMachine.ChangeState(new PatrolState(enemy));
            }
            return;
        }

        enemy.FaceTarget(target);

        // 1. Ưu tiên Melee Attack
        if (senses.IsInMeleeRange(target) && combat.CanMeleeAttack())
        {
            stateMachine.ChangeState(new MeleeAttackState(enemy));
            return;
        }

        // 2. Nếu đã trong tầm bắn (kể cả đang cooldown) → ĐỨNG YÊN, không tiến thêm
        if (senses.IsInShootingRange(target))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (combat.CanRangedAttack())
            {
                stateMachine.ChangeState(new RangedAttackState(enemy));
            }
            return; // dù bắn được hay đang cooldown, đều KHÔNG tiến lại gần
        }

        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);
        if (distance <= config.turnThreshold)
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }

        if (senses.IsHittingWall() || senses.IsAtCliff())
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }

        rb.linearVelocity = new Vector2(config.chaseSpeed * enemy.FacingDirection, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        rb.linearVelocity = Vector2.zero;
    }
}