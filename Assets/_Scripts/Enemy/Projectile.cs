using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask targetLayer;

    public float lifetime = 5f;
    public int damage = 1;
    public float knockbackForce = 0f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }

        if (((1 << collision.gameObject.layer) & targetLayer) == 0)
        {
            return;
        }

        Health health = collision.GetComponentInChildren<Health>();
        if (health != null)
        {
            health.ChangeHealth(-damage, transform.position, knockbackForce);
        }

        Destroy(gameObject);
    }
}