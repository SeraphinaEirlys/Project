using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    public Animator anim;
    public Health health;
    public Rigidbody2D rb;
    public Entity_VFX vfx;

    public float deathDelay = 1f;

    [Header("Knockback & Stun Defaults")]
    public float defaultKnockbackDuration = 0.15f;
    
    private Vector2 pendingKnockbackForce;
    private float pendingStunDuration = 0.8f;

    public void SetPendingHitData(Vector2 knockbackForce, float stunDuration)
    {
        pendingKnockbackForce = knockbackForce;
        pendingStunDuration = stunDuration;
    }

    public void ApplyKnockback(Vector2 force)
    {
        pendingKnockbackForce = force;
    }

    public void SetPendingStun(float duration)
    {
        pendingStunDuration = duration;
    }

    private void Awake()
    {
        if (rb == null) TryGetComponent(out rb);
        if (vfx == null) TryGetComponent(out vfx);
        if (enemy == null) TryGetComponent(out enemy);
    }

    private void OnEnable()
    {
        health.OnDamaged += HandleDamage;
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDamaged -= HandleDamage;
        health.OnDeath -= HandleDeath;
    }

    void HandleDamage(Vector2 sourcePosition, float knockbackForce)
    {
        vfx?.PlayOnDamageVfx();

        if (enemy != null && enemy.StateMachine != null)
        {
            enemy.StateMachine.ChangeState(new DamagedState(
                enemy,
                pendingKnockbackForce,
                defaultKnockbackDuration,
                pendingStunDuration
            ));

            pendingKnockbackForce = Vector2.zero;
        }
    }

    void HandleDeath(Vector2 sourcePosition, float knockbackForce)
    {
        anim.SetTrigger("isDead");
        GetComponent<Collider2D>().enabled = false;

        if (enemy != null)
        {
            enemy.Die();
        }

        enabled = false;
        Destroy(gameObject);
    }
}