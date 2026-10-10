using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    private EnemyConfig config;
    private Enemy enemy;
    private float lastAttackTime;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        config = enemy.Config;
    }

    public bool CanMeleeAttack() => Time.time >= lastAttackTime + config.meleeCooldown;

    public bool CanRangedAttack() => Time.time >= lastAttackTime + config.rangedCooldown;

    public void PerformMeleeAttack()
    {
        Debug.Log($"[PerformMeleeAttack] Called at {Time.time}");

        if (!enemy.CanAct) return;

        lastAttackTime = Time.time;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, config.meleeRange, playerLayer);

        if (!hit)
            return;

        Health health = hit.GetComponentInChildren<Health>();

        if (health != null)
            health.ChangeHealth(-config.meleeDamage, transform.position, config.knockbackForce);
    }

    public void PerformRangedAttack()
    {
        lastAttackTime = Time.time;

        if (enemy.CurrentTarget == null) return;

        Vector2 targetPos = enemy.CurrentTarget.position;
        Collider2D playerCol = enemy.CurrentTarget.GetComponentInChildren<Collider2D>();

        if (playerCol != null)
        {
            targetPos = playerCol.bounds.center;
        }
        else
        {
            targetPos += Vector2.up * 1f;
        }

        Vector2 fireDirection = (targetPos - (Vector2)attackPoint.position).normalized;

        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject newProjectile = Instantiate(config.projectilePrefab, attackPoint.position, rotation);

        Projectile projectileScript = newProjectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.damage = config.rangedDamage;
            projectileScript.lifetime = config.projectileLifetime;
            projectileScript.knockbackForce = config.knockbackForce; // truyền knockback riêng của quái vào đạn
        }

        Rigidbody2D rbProj = newProjectile.GetComponent<Rigidbody2D>();
        if (rbProj != null)
        {
            rbProj.linearVelocity = fireDirection * config.projectileSpeed;
        }
    }
}