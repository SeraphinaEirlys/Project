using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Variables
    public int FacingDirection { get; private set; } = 1;
    public Transform CurrentTarget { get; set; }

    // Components
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public EnemyConfig Config;
    public StateMachine StateMachine { get; private set; }
    public EnemySenses Senses { get; private set; }
    public Enemy_Combat Combat { get; private set; }
    public bool CanAct { get; set; } = true;
    public Vector2 StartingPoint { get; private set; }

    public PersistentGUID persistentGUID { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        StateMachine = new StateMachine();
        Senses = GetComponent<EnemySenses>();
        Combat = GetComponent<Enemy_Combat>();

        persistentGUID = GetComponent<PersistentGUID>();
    }

    public void Start()
    {
        if (persistentGUID != null && WorldState.Instance != null && WorldState.Instance.defeatedEnemies.Contains(persistentGUID.GUID))
        {
            Destroy(gameObject);
            return;
        }

        StartingPoint = transform.position;
        if (Config != null && Config.isStationary)
        {
            StateMachine.Initialize(new IdleState(this));
        }
        else
        {
            StateMachine.Initialize(new PatrolState(this));
        }
    }

    private void Update() => StateMachine.CurrentState?.Update();
    private void FixedUpdate() => StateMachine.CurrentState?.FixedUpdate();
    public void OnAnimationFinished() => StateMachine.CurrentState?.OnAnimationFinished();

    public void FaceTarget(Transform target)
    {
        float offset = target.position.x - transform.position.x;

        int direction = offset > 0 ? 1 : -1;
        if (direction != FacingDirection)
        {
            Flip();
        }
    }

    public void Die()
    {
        if (persistentGUID != null && WorldState.Instance != null)
        {
            WorldState.Instance.defeatedEnemies.Add(persistentGUID.GUID);
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