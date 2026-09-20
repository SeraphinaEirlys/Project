using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    public Animator anim;
    public Health health;
    public Rigidbody2D rb;
    public Entity_VFX vfx;

    public float deathDelay = 1f;


    public void ApplyKnockback(Vector2 force)
    {
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }


    private void Awake()
    {
        if (rb == null)
        {
            TryGetComponent(out rb);
        }

        if (vfx == null)
        {
            TryGetComponent(out vfx);
        }
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

    void HandleDamage(Vector2 sourcePosition)
    {
        int knockbackDir = 0;
        knockbackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        vfx?.PlayOnDamageVfx();

        enemy.StateMachine.ChangeState(new DamagedState(enemy, knockbackDir));
    }

    void HandleDeath()
    {
        anim.SetTrigger("isDead");

        GetComponent<Collider2D>().enabled = false;
        enabled = false;

        Destroy(gameObject, deathDelay);
    }
}
