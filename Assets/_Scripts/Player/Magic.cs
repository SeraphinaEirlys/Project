using System;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public Player player;
    
    private int currentPrayers;

    public event Action OnPrayersChanged;

    public int CurrentPrayers => currentPrayers;
    public int MaxPrayers => ProgressionManager.Instance != null ? ProgressionManager.Instance.maxPrayers : 0;

    private void Start()
    {
        if (ProgressionManager.Instance != null)
        {
            currentPrayers = ProgressionManager.Instance.maxPrayers;
            ProgressionManager.Instance.OnStatsChanged += HandleStatsChanged;
        }
        OnPrayersChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (ProgressionManager.Instance != null)
            ProgressionManager.Instance.OnStatsChanged -= HandleStatsChanged;
    }

    private void HandleStatsChanged()
    {
        if (ProgressionManager.Instance != null)
            currentPrayers = ProgressionManager.Instance.maxPrayers;
        OnPrayersChanged?.Invoke();
    }

    public void RefillPrayers()
    {
        if (ProgressionManager.Instance != null)
        {
            currentPrayers = ProgressionManager.Instance.maxPrayers;
            OnPrayersChanged?.Invoke();
        }
    }

    public bool TryCastHeal()
    {
        if (ProgressionManager.Instance == null) return false;

        if (currentPrayers > 0 && player.health.health < ProgressionManager.Instance.maxHealth)
        {
            currentPrayers--;
            OnPrayersChanged?.Invoke();
            player.ChangeState(player.spellcastState);
            return true;
        }
        return false;
    }

    public void CastSpell()
    {
        if (ProgressionManager.Instance == null) return;

        int potency = ProgressionManager.Instance.prayerPotency;
        player.health.ChangeHealth(potency, player.transform.position);
        
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound2D("Heal");
    }

    public int GetCurrentPrayers() => currentPrayers;
}