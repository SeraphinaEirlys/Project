using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Knockback Settings")]
    public float knockbackForce = 20;
    public float knockbackDuration = .2f;

    public Health health;

    public Entity_VFX vfx;

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


        player.damagedState.SetParameter(knockbackDir);
        player.ChangeState(player.damagedState);
    }

    void HandleDeath()
    {
        
    }
}
