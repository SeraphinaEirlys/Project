using UnityEngine;

public class ReturnState : State
{
    // Sử dụng animation di chuyển (isWalking hoặc isRunning tùy cấu hình Animator của bạn)
    protected override string AnimBoolName => "isWalking";

    public ReturnState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // 1. Nếu trên đường rút lui mà người chơi quay lại khu vực (lọt vào tầm phát hiện) -> Quay lại truy đuổi tiếp
        Transform target = senses.GetChaseTarget();
        if (target != null)
        {
            stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }

        // 2. Xác định hướng quay mặt về vị trí chốt gác ban đầu
        float dirX = enemy.StartingPoint.x - enemy.transform.position.x;
        if (Mathf.Abs(dirX) > 0.1f)
        {
            int targetFacing = dirX > 0 ? 1 : -1;
            if (targetFacing != enemy.FacingDirection)
            {
                enemy.Flip();
            }
        }

        // 3. Di chuyển về điểm StartingPoint
        // Phân biệt: Quái bay (Gravity = 0) di chuyển cả X và Y; Quái mặt đất chỉ di chuyển theo trục X
        if (rb.gravityScale == 0)
        {
            // Dành cho quái bay: bay mượt mà theo cả 2 trục về tổ
            Vector2 currentPos = enemy.transform.position;
            Vector2 nextPos = Vector2.MoveTowards(currentPos, enemy.StartingPoint, config.patrolSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);
        }
        else
        {
            // Dành cho lính gác mặt đất: chỉ chạy ngang theo trục X
            rb.linearVelocity = new Vector2(config.patrolSpeed * enemy.FacingDirection, rb.linearVelocity.y);
        }

        // 4. Khi đã về tới điểm gác ban đầu -> Chuyển về IdleState đứng canh
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