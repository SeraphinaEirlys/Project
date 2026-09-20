using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private int maxCombo;
    private int comboStep;
    private bool isRisingCombo;
    private bool comboWindowOpen;

    public PlayerAttackState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();

        bool isContinuation = player.HasComboMemory;

        if (isContinuation)
        {
            comboStep     = player.comboMemoryStep + 1;
            isRisingCombo = player.comboMemoryIsRising;
            player.ResetComboMemory();
        }
        else
        {
            comboStep     = 1;
            isRisingCombo = MoveInput.y > .1f;
        }

        maxCombo = isRisingCombo ? 2 : 3;
        if (comboStep > maxCombo) comboStep = 1;

        comboWindowOpen = false;

        player.attackBuffered = false;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        anim.SetBool("isRisingCombo", isRisingCombo);
        anim.SetInteger("comboStep", comboStep);
        anim.SetTrigger("Attack");

        player.LockFacing();
    }

    public override void Update()
    {
        base.Update();

        if (!comboWindowOpen) return;
        if (comboStep >= maxCombo) { comboWindowOpen = false; return; }

        if (player.ConsumeAttackBuffer())
        {
            comboWindowOpen = false;

            player.attackBuffered = false;

            comboStep++;

            anim.SetInteger("comboStep", comboStep);
            anim.SetTrigger("NextAttack");
        }
    }

    public void EnableComboWindow()
    {
        if (comboStep >= maxCombo) return;
        comboWindowOpen = true;
    }

    public void DisableComboWindow()
    {
        comboWindowOpen = false;
    }

    public override void AttackAnimationFinish()
    {
        if (comboStep < maxCombo)
        {
            player.SaveComboMemory(comboStep, isRisingCombo, player.comboWindowDuration);
        }
        else
        {
            player.ResetComboMemory();
            player.TriggerAttackCooldown();
        }

        if (Mathf.Abs(MoveInput.x) > .1f)
            player.ChangeState(player.moveState);
        else
            player.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        comboWindowOpen = false;

        anim.ResetTrigger("Attack");
        anim.ResetTrigger("NextAttack");
        anim.SetBool("isRisingCombo", false);

        player.UnlockFacing();
    }
}