using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Combat combat;
    protected Damage damage;
    protected Magic magic;

    protected bool JumpPressed {get => player.jumpPressed; set => player.jumpPressed = value;}
    protected bool JumpReleased {get => player.jumpReleased; set => player.jumpReleased = value;}
    protected bool RunPressed => player.runPressed;
    protected bool AttackPressed => player.attackPressed;
    protected Vector2 MoveInput => player.moveInput;
    protected bool SpellcastPressed => player.spellcastPressed;

    public PlayerState (Player player)
    {
        this.player = player;
        this.anim = player.anim;
        this.rb = player.rb;
        combat = player.combat;
        magic = player.magic;
        damage = player.damage;
    }


    public virtual void Enter() {}
    public virtual void Exit() {}

    public virtual void Update() {}
    public virtual void FixedUpdate() {}
    public virtual void AnimationFinished() {}
    public virtual void AttackAnimationFinish() {}
    public virtual void WallJumpLaunch() {}
}
