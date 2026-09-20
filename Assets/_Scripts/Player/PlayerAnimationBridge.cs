using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void EnableComboWindow()
    {
        if (player != null && player.currentState is PlayerAttackState attackState)
        {
            attackState.EnableComboWindow();
        }
    }

    public void TriggerAttack(int hitboxIndex)
    {
        if (player != null && player.combat != null)
        {
            player.combat.Attack(hitboxIndex);
        }
    }

    public void AttackAnimationFinished()
    {
        Debug.Log("AttackAnimationFinished fired! Current state: " + player.currentState.GetType().Name);

        if (player != null)
        {
            player.AttackAnimationFinished();
        }
    }

    public void PlayerAnimationFinished()
    {
        if (player != null)
        {
            player.AnimationFinished();
        }
    }

    public void EnableAirComboWindow()
    {
        if (player != null && player.currentState is PlayerAirAttackState airAttackState)
            airAttackState.EnableComboWindow();
    }

    public void DisableAirComboWindow()
    {
        if (player != null && player.currentState is PlayerAirAttackState airAttackState)
            airAttackState.DisableComboWindow();
    }

    public void FreezeMovement()
    {
        if (player != null && player.currentState is PlayerAirAttackState airAttackState)
        {
            airAttackState.FreezeMovement();
        }
    }

    public void UnfreezeMovement()
    {
        if (player != null && player.currentState is PlayerAirAttackState airAttackState)
        {
            airAttackState.UnfreezeMovement();
        }
    }

    public void WallJumpLaunch()
    {
        if (player != null)
        {
            player.WallJumpLaunch();
        }
    }

    public void LockFacing()
    {
        if (player != null)
        {
            player.LockFacing();
        }
    }

    public void UnlockFacing()
    {
        if (player != null)
        {
            player.UnlockFacing();
        }
    }
}