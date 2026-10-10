using UnityEngine;

public class MeleeAttackState : State
{
    protected override string AnimBoolName => "isAttacking";

    public MeleeAttackState(Enemy enemy) : base(enemy){}

    public override void Enter()
    {
        base.Enter();

        rb.linearVelocity = Vector2.zero;
    }

    public override void OnAnimationFinished()
    {
        float recoveryDuration = 0.6f; 

        stateMachine.ChangeState(new IdleState(enemy, recoveryDuration));
    }
}