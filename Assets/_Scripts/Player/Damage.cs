using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Knockback Settings")]
    public float knockbackDuration = .2f; // chỉ còn giữ thời gian phục hồi, lực đến từ kẻ tấn công

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

    void HandleDamage(Vector2 sourcePosition, float knockbackForce)
    {
        int knockbackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        vfx?.PlayOnDamageVfx();

        player.damagedState.SetParameter(knockbackDir, knockbackForce);
        player.ChangeState(player.damagedState);
    }

    void HandleDeath(Vector2 sourcePosition, float knockbackForce)
    {
        int knockbackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        player.deathState.SetParameters(knockbackDir, knockbackForce);
        player.ChangeState(player.deathState);
    }
}