using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Variables
    public int FacingDirection{get; private set;} = 1;

    //Components
    public Rigidbody2D RB {get; private set;}
    public Animator Anim{get; private set;}
    public EnemyConfig Config;
    public StateMachine StateMachine{get; private set;}
    public EnemySenses Senses{get; private set;}
    public Enemy_Combat Combat {get; private set;}

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        StateMachine = new StateMachine();
        Senses = GetComponent<EnemySenses>();
        Combat = GetComponent<Enemy_Combat>();
    }

    public void Start()
    {
        StateMachine.Initialize(new PatrolState(this));
    }

    private void Update() => StateMachine.CurrentState?.Update();
    private void FixedUpdate() => StateMachine.CurrentState?.FixedUpdate();
    public void OnAnimationFinished() => StateMachine.CurrentState?.OnAnimationFinished();

    public void FaceTarget(Transform target)
    {
        float offset = target.position.x - transform.position.x;

        int direction = offset > 0 ? 1 : -1;
        if(direction != FacingDirection)
        {
            Flip();
        }
    }

    public void Flip()
    {
        FacingDirection *= -1;

        Vector3 scale = transform.localScale;
        scale.x = FacingDirection;
        transform.localScale = scale;
    }
}
