using UnityEngine;

public class PlayerStatsApplier : MonoBehaviour
{
    [SerializeField] private Health health;
    private ProgressionManager progression;

    private void Reset() => health = GetComponent<Health>();

    private void Start()
    {
        progression = ProgressionManager.Instance;
        if (progression == null || health == null) return;
        
        progression.OnStatsChanged += HandleStatsChanged;
        
        Apply(fillHealth: true);
    }

    private void OnDestroy()
    {
        if (progression != null)
            progression.OnStatsChanged -= HandleStatsChanged;
    }

    private void HandleStatsChanged() => Apply(fillHealth: false);

    private void Apply(bool fillHealth)
    {
        int newMax = progression.maxHealth;
        int increase = Mathf.Max(0, newMax - health.maxHealth);

        health.maxHealth = newMax;

        if (fillHealth)
        {
            health.health = newMax;
        }
        else
        {
            health.health = Mathf.Min(health.health + increase, newMax); 
        }
    }
}