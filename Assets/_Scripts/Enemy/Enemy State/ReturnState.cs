using UnityEngine;

public class ReturnState : State
{
    protected override string AnimBoolName => "isWalking";

    public ReturnState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Transform target = senses.GetChaseTarget();
        if (target != null)
        {
            stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }

        float dirX = enemy.StartingPoint.x - enemy.transform.position.x;
        if (Mathf.Abs(dirX) > 0.1f)
        {
            int targetFacing = dirX > 0 ? 1 : -1;
            if (targetFacing != enemy.FacingDirection)
            {
                enemy.Flip();
            }
        }

        if (rb.gravityScale == 0)
        {
            Vector2 currentPos = enemy.transform.position;
            Vector2 nextPos = Vector2.MoveTowards(currentPos, enemy.StartingPoint, config.patrolSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);
        }
        else
        {
            rb.linearVelocity = new Vector2(config.patrolSpeed * enemy.FacingDirection, rb.linearVelocity.y);
        }

        float distanceToSpawn = Vector2.Distance(enemy.transform.position, enemy.StartingPoint);
        if (distanceToSpawn <= 0.2f)
        {
            rb.linearVelocity = Vector2.zero;
            stateMachine.ChangeState(new IdleState(enemy));
        }
    }

    public override void Exit()
    {
        base.Exit();
        rb.linearVelocity = Vector2.zero;
    }
}