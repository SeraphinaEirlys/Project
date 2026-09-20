using UnityEngine;

public class IdleState : State
{
    private Transform target;
    protected override string AnimBoolName => "isIdling";

    public IdleState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        target = senses.GetChaseTarget();

        if (!target)
        {
            stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        enemy.FaceTarget(target);

        if (senses.IsInMeleeRange(target) && combat.CanMeleeAttack())
        {
            stateMachine.ChangeState(new MeleeAttackState(enemy));
            return;
        }

        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);

        if (distance <= config.turnThreshold)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (senses.IsHittingWall() || senses.IsAtCliff())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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