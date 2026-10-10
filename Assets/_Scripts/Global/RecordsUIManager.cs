using UnityEngine;
using TMPro;

public class RecordsUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playtimeText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text prayerText;
    [SerializeField] private TMP_Text prayerPotencyText;

    [SerializeField] private Health playerHealth;
    [SerializeField] private Magic playerMagic;

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshRecordsUI();
        }
    }

    public void RefreshRecordsUI()
    {
        if (PlaytimeTracker.Instance != null)
        {
            playtimeText.text = "Playtime: " + PlaytimeTracker.Instance.GetFormattedPlaytime();
        }

        if (ProgressionManager.Instance != null)
        {
            if (playerHealth != null)
            {
                hpText.text = $"HP: {playerHealth.health} / {ProgressionManager.Instance.maxHealth}";
            }

            attackText.text = $"Attack: {ProgressionManager.Instance.attackPower}";

            if (playerMagic != null)
            {
                int current = playerMagic.CurrentPrayers;
                int max = playerMagic.MaxPrayers;
                prayerText.text = $"Prayers: {current} / {max}";
            }
            else
            {
                int maxPrayers = ProgressionManager.Instance.maxPrayers;
                prayerText.text = $"Prayers: {maxPrayers} / {maxPrayers}";
            }
            
            prayerPotencyText.text = $"Prayer Potency: {ProgressionManager.Instance.prayerPotency}";
        }
    }
}