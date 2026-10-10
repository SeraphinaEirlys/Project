using UnityEngine;

public class RangedAttackState : State
{
    protected override string AnimBoolName => "isShooting";
    private bool attackFinished;

    public RangedAttackState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        base.Enter();

        attackFinished = false;
        rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        if (!attackFinished) return;

        if (senses.GetChaseTarget() != null)
        {
            stateMachine.ChangeState(new ChaseState(enemy));
        }
        else
        {
            stateMachine.ChangeState(new IdleState(enemy));
        }
    }

    public override void OnAnimationFinished()
    {
        base.OnAnimationFinished();
        attackFinished = true;
    }
}