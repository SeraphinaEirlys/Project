using UnityEngine;
using System.Collections.Generic;

public class Combat : MonoBehaviour
{
    [System.Serializable]
    public class AttackHitbox
    {
        public string name;
        
        [Header("Damage")]
        public int damage = 10; // Sát thương cơ bản của riêng đòn đánh này

        [Header("Knockback Settings")]
        public bool applyKnockback = false;
        public Vector2 knockbackForce = new Vector2(5f, 2f);

        [Header("Stun Settings")]
        public float stunDuration = 0.8f;

        [Header("Gizmo Settings")]
        public bool showGizmo = true;
        public Color gizmoColor = Color.red;

        [Header("Transform")]
        public Vector2 size = new Vector2(1.2f, 0.8f);
        public Vector2 offset = new Vector2(0.7f, 0f);
        public float angle = 0f;
    }

    [Header("Hitbox Configurations")]
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public GameObject hitFXPrefab;
    public AttackHitbox[] hitboxes;

    [Header("Gizmos Preview")]
    public bool showAllHitboxes = true;
    public int previewHitboxIndex = -1;

    public Player player;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<Player>();
    }

    public void Attack(int hitboxIndex)
    {
        if (hitboxes == null || hitboxIndex < 0 || hitboxIndex >= hitboxes.Length)
        {
            Debug.LogWarning($"Hitbox index {hitboxIndex} không tồn tại!");
            return;
        }

        AttackHitbox hitbox = hitboxes[hitboxIndex];

        int dir = player != null ? player.facingDirection : 1;
        Vector2 origin = (Vector2)attackPoint.position + new Vector2(hitbox.offset.x * dir, hitbox.offset.y);
        float currentAngle = hitbox.angle * dir;

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayer);
        contactFilter.useTriggers = true;

        Collider2D[] results = new Collider2D[10];
        int hitCount = Physics2D.OverlapBox(origin, hitbox.size, currentAngle, contactFilter, results);

        if (hitCount > 0)
        {
            Vector2 centerPosition = origin;
            Vector2 fxPosition = centerPosition;
            
            if (hitCount > 1)
            {
                Vector2 averagePos = Vector2.zero;
                int validCount = 0;
                
                for (int i = 0; i < hitCount; i++)
                {
                    if (results[i] != null)
                    {
                        Vector2 enemyCenter = results[i].bounds.center;
                        Vector2 halfSize = hitbox.size * 0.5f;
                        Vector2 localPoint = Quaternion.Inverse(Quaternion.Euler(0, 0, currentAngle)) * (enemyCenter - centerPosition);
                        if (Mathf.Abs(localPoint.x) <= halfSize.x && Mathf.Abs(localPoint.y) <= halfSize.y)
                        {
                            averagePos += enemyCenter;
                            validCount++;
                        }
                    }
                }
                
                if (validCount > 0)
                {
                    fxPosition = averagePos / validCount;
                }
            }
            else if (hitCount == 1 && results[0] != null)
            {
                fxPosition = results[0].bounds.center;
            }

            if (hitFXPrefab != null)
            {
                GameObject fxInstance = Instantiate(hitFXPrefab, fxPosition, Quaternion.identity);
                Animator fxAnimator = fxInstance.GetComponent<Animator>();
                
                if (fxAnimator != null)
                {
                    fxAnimator.Play("HitFX", 0, 0f);
                    AnimatorStateInfo stateInfo = fxAnimator.GetCurrentAnimatorStateInfo(0);
                    Destroy(fxInstance, stateInfo.length + 0.1f);
                }
                else
                {
                    Destroy(fxInstance, 1f);
                }
            }

            HashSet<Enemy_Damage> processedEnemies = new HashSet<Enemy_Damage>();

            for (int i = 0; i < hitCount; i++)
            {
                if (results[i] != null)
                {
                    Enemy_Damage enemyScript = results[i].GetComponentInParent<Enemy_Damage>();
                    if (enemyScript != null && !processedEnemies.Contains(enemyScript))
                    {
                        processedEnemies.Add(enemyScript);
                        
                        // Tính toán hướng & lực knockback
                        Vector2 finalKnockback = Vector2.zero;
                        if (hitbox.applyKnockback)
                        {
                            finalKnockback = new Vector2(hitbox.knockbackForce.x * dir, hitbox.knockbackForce.y);
                        }

                        // Set cả Knockback lẫn Stun TRƯỚC KHI gọi ChangeHealth
                        enemyScript.SetPendingHitData(finalKnockback, hitbox.stunDuration);

                        // TÍNH TOÁN SÁT THƯƠNG: base × (100 + attack)% 
                        int finalDamage = hitbox.damage;
                        if (ProgressionManager.Instance != null)
                        {
                            finalDamage = Mathf.RoundToInt(
                                hitbox.damage * (100 + ProgressionManager.Instance.attackPower) / 100f
                            );
                        }

                        Health hp = results[i].GetComponentInParent<Health>();
                        if (hp != null)
                        {
                            hp.ChangeHealth(-finalDamage, transform.position);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showAllHitboxes || attackPoint == null || hitboxes == null || hitboxes.Length == 0)
            return;

        int dir = player != null ? player.facingDirection : 1;

        for (int i = 0; i < hitboxes.Length; i++)
        {
            var hb = hitboxes[i];

            if (previewHitboxIndex >= 0)
            {
                if (previewHitboxIndex != i) continue;
            }
            else if (!hb.showGizmo)
            {
                continue;
            }

            Vector2 origin = (Vector2)attackPoint.position + new Vector2(hb.offset.x * dir, hb.offset.y);
            float angle = hb.angle * dir;

            Gizmos.color = (previewHitboxIndex == i) ? Color.green : hb.gizmoColor;

            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.Euler(0, 0, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, hb.size);
            Gizmos.matrix = oldMatrix;
        }
    }
}