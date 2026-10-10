using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashDirection;
    private float originalGravity;
    private float dashTimer;
    private float dashStopTimer;
    private bool isStopping;
    private float afterImageTimer;

    [Header("VFX")]
    public GameObject dashSmokePrefab;

    public PlayerDashState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();

        dashTimer = player.dashDuration;
        dashStopTimer = 0f;
        isStopping = false;

        afterImageTimer = player.afterImageCooldown;

        anim.SetBool("isDashing", true);

        if (Mathf.Abs(player.moveInput.x) > 0.1f)
            dashDirection = (int)Mathf.Sign(player.moveInput.x);
        else
            dashDirection = player.facingDirection;

        if (player.dashSmokePrefab != null)
        {
            GameObject smoke = GameObject.Instantiate(player.dashSmokePrefab, player.transform.position, Quaternion.identity);

            Vector3 smokeScale = smoke.transform.localScale;
            smokeScale.x = -dashDirection;
            smoke.transform.localScale = smokeScale;
        }

        originalGravity = player.rb.gravityScale;
        player.rb.gravityScale = 0f;

        player.LockFacing();

        if (player.health != null) player.health.isInvincible = true;
    }

    public override void Update()
    {
        base.Update();

        if (!player.isGrounded && rb.linearVelocity.y < -0.1f && dashTimer <= 0)
        {
            player.ChangeState(player.fallState);
            return;
        }

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;

            afterImageTimer -= Time.deltaTime;
            if (afterImageTimer <= 0f)
            {
                SpawnAfterImage();
                afterImageTimer = player.afterImageCooldown;
            }

            if (dashTimer <= 0 && player.health != null)
            {
                player.health.isInvincible = false;
            }
        }
        else if (!isStopping)
        {
            if (Mathf.Abs(player.moveInput.x) > .1f && player.isGrounded)
            {
                player.ChangeState(player.moveState);
                return;
            }

            isStopping = true;
            dashStopTimer = player.dashStopDuration;
        }
        else
        {
            dashStopTimer -= Time.deltaTime;

            if (Mathf.Abs(player.moveInput.x) > .1f && player.isGrounded)
            {
                player.ChangeState(player.moveState);
                return;
            }

            if (dashStopTimer <= 0)
            {
                if (player.isGrounded)
                    player.ChangeState(player.idleState);
                else
                    player.ChangeState(player.fallState);
            }
        }
    }

    private void SpawnAfterImage()
    {
        if (player.afterImagePrefab == null) return;

        SpriteRenderer playerSr = player.GetComponentInChildren<SpriteRenderer>();
        if (playerSr == null) return;

        GameObject afterImage = GameObject.Instantiate(
            player.afterImagePrefab,
            playerSr.transform.position,
            Quaternion.identity
        );

        SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sprite = playerSr.sprite;

            sr.sortingLayerID = playerSr.sortingLayerID;
            sr.sortingOrder = playerSr.sortingOrder - 1;
        }

        Vector3 scale = afterImage.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -dashDirection;
        afterImage.transform.localScale = scale;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (dashTimer > 0)
        {
            player.rb.linearVelocity = new Vector2(player.dashSpeed * dashDirection, 0f);
        }
        else
        {
            player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("isDashing", false);
        player.UnlockFacing();

        player.rb.gravityScale = originalGravity;
        player.dashCooldownTimer = player.dashCooldown;

        if (player.health != null) player.health.isInvincible = false;
    }
}