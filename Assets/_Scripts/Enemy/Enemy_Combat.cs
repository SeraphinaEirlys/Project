using System;
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

    public void PerformMeleeAttack()
    {
        lastAttackTime = Time.time;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, config.meleeRange, playerLayer);

        Debug.Log(hit);

        if(!hit)
            return;

        Health health = hit.GetComponentInChildren<Health>();

        if(health != null)
            health.ChangeHealth(-config.meleeDamage, transform.position);
    }
}
