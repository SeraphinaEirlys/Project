using UnityEngine;

public class PlayerAirAttackState : PlayerState
{
    private const int MaxAirCombo = 3;
    private int comboStep;
    private bool canCombo;

    private float comboWindowDuration = 1f;
    private float comboWindowTimer = 0f;

    private float uppercutForce = 6f;
    private float downslamSpeed = 25f;
    private bool isDownslamming = false;
    private bool hasLanded = false;
    private bool isFrozen = false;

    public PlayerAirAttackState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();

        comboStep = 1;
        isDownslamming = false;
        hasLanded = false;

        ApplyMovement(comboStep);

        anim.SetInteger("airComboStep", comboStep);
        anim.SetTrigger("AirAttack");

        EnableComboWindow();
    }

    public override void Update()
    {
        base.Update();

        if (canCombo)
        {
            comboWindowTimer += Time.deltaTime;
            
            if (comboWindowTimer >= comboWindowDuration)
            {
                canCombo = false;
                player.attackBuffered = false;
            }
        }

        if (isFrozen) return;

        if (comboStep == 3 && isDownslamming)
        {
            if (!hasLanded && player.isGrounded)
            {
                hasLanded = true;
                isDownslamming = false;
                
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f;
                
                anim.SetTrigger("SlamImpact");
            }
        }

        if (canCombo && comboStep < MaxAirCombo && player.ConsumeAirAttackBuffer())
        {
            comboStep++;
            canCombo = false;
            comboWindowTimer = 0f;
            isDownslamming = false;
            hasLanded = false;

            ApplyMovement(comboStep);

            anim.SetInteger("airComboStep", comboStep);
            anim.SetTrigger("NextAirAttack");

            EnableComboWindow();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (comboStep == 3 && isDownslamming && !hasLanded)
        {
            rb.linearVelocity = new Vector2(0f, -downslamSpeed);
        }
    }

    private void ApplyMovement(int step)
    {
        switch (step)
        {
            case 1:
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f;
                break;

            case 2:
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(0f, uppercutForce);
                break;

            case 3:
                isDownslamming = true;
                hasLanded = false;
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(0f, -downslamSpeed);
                anim.SetBool("isDownslamFalling", true);
                break;
        }
    }

    public void FreezeMovement()
    {
        isFrozen = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    public void UnfreezeMovement()
    {
        isFrozen = false;

        if (comboStep == 3 && isDownslamming && !hasLanded)
        {
            rb.linearVelocity = new Vector2(0f, -downslamSpeed);
        }
    }

    public void EnableComboWindow() 
    {
        canCombo = true;
        comboWindowTimer = 0f;
    }

    public void DisableComboWindow() 
    {
        canCombo = false;
    }

    public void StartDownslamDirectly()
    {
        comboStep = 3;
        canCombo = false;
        comboWindowTimer = 0f;

        player.slideTriggered = false;

        ApplyMovement(3);

        anim.SetInteger("airComboStep", 3);
        anim.SetTrigger("AirAttack"); 
    }

    public void OnSlamImpactFinished()
    {
        if (player.isGrounded)
            player.ChangeState(player.idleState);
        else
            player.ChangeState(player.fallState);
    }

    public override void AttackAnimationFinish()
    {
        if (player.isGrounded)
            player.ChangeState(player.idleState);
        else
            player.ChangeState(player.fallState);
    }

    public override void Exit()
    {
        base.Exit();
        
        rb.gravityScale = player.normalGravity;
        anim.ResetTrigger("AirAttack");
        anim.ResetTrigger("NextAirAttack");
        anim.ResetTrigger("SlamImpact");
        anim.SetBool("isDownslamFalling", false);
        
        canCombo = false;
        comboWindowTimer = 0f;
        player.attackBuffered = false;

        player.slideTriggered = false;

        isDownslamming = false;
        hasLanded = false;

        player.TriggerAirAttackCooldown();
    }
}