using System;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    [Header("Core Stats (Mặc định khi mới chơi)")]
    public int maxHealth = 70;

    [Tooltip("Phần trăm bonus damage. 0 = 100% base, 30 = 130% base, 100 = 200% base")]
    public int attackPower = 0;

    [Header("Prayer Stats")]
    public int maxPrayers = 6;
    public int prayerPotency = 26;

    public event Action OnStatsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddAttack(int amount)
    {
        attackPower += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddMaxPrayers(int amount)
    {
        maxPrayers += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddPrayerPotency(int amount)
    {
        prayerPotency += amount;
        OnStatsChanged?.Invoke();
    }
}