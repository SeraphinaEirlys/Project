using UnityEngine;

public class PlayerDamagedState : PlayerState
{
    private float knockbackVelocity;
    private bool isAnimationFinished;

    private float safetyTimer;
    private const float maxDuration = 1f;

    public PlayerDamagedState(Player player) : base(player) {}

    public void SetParameter(int knockbackDirection, float knockbackForce)
    {
        knockbackVelocity = knockbackDirection * knockbackForce;

        int faceDirection = knockbackDirection > 0 ? -1 : 1;
        player.facingDirection = faceDirection;
        player.transform.localScale = new Vector3(faceDirection, 1, 1);
    }

    public override void Enter()
    {
        base.Enter();

        isAnimationFinished = false;
        safetyTimer = 0f;
        anim.SetBool("isDamaged", true);

        player.LockFacing();

        player.rb.linearVelocity = new Vector2(knockbackVelocity, player.rb.linearVelocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float currentX = player.rb.linearVelocity.x;
        float newX = Mathf.MoveTowards(currentX, 0, 30f * Time.fixedDeltaTime);

        player.rb.linearVelocity = new Vector2(newX, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        safetyTimer += Time.deltaTime;

        if (isAnimationFinished || safetyTimer >= maxDuration)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void AnimationFinished()
    {
        base.AnimationFinished();
        isAnimationFinished = true;
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("isDamaged", false);
        player.UnlockFacing();

        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }
}