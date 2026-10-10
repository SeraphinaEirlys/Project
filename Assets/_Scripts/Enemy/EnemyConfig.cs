using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("General")]
    public float turnThreshold = .2f;

    [Header("Movement")]
    public float patrolSpeed = 5;
    public bool isStationary = false;

    [Header("Patrol")]
    public float groundCheckDistance = .7f;
    public float wallCheckDistance = .7f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    
    [Header("Chase")]
    public float chaseSpeed = 7;
    public float chaseRange = 5;
    public LayerMask targetLayer;
    public bool returnsToStart = false;

    [Header("Attack")]
    public float meleeRange = 1.2f;
    public int meleeDamage = 2;
    public float meleeCooldown = 1;

    [Header("Ranged Attack")]
    public float rangedRange = 5f;
    public int rangedDamage = 1;
    public float rangedCooldown = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;
    public float projectileLifetime = 5f;

    [Header("Damaged")]
    public float knockbackDuration = .2f;
    public float knockbackForce = 15;
}