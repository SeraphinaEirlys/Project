using UnityEngine;

public class PatrolState : State
{
    protected override string AnimBoolName => "isWalking";

    public PatrolState(Enemy enemy) : base(enemy){}

    public override void Enter()
    {
        base.Enter();

        if (config.isStationary)
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (senses.GetChaseTarget())
        {
            stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }

        if(senses.IsHittingWall() || senses.IsAtCliff())
        {
            enemy.Flip();
            Debug.Log("I found a cliff cuh");
            return;
        }

        rb.linearVelocity = new Vector2(config.patrolSpeed * enemy.FacingDirection, rb.linearVelocity.y);
    }


}
