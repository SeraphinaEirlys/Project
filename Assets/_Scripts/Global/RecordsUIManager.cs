using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RecordsUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playtimeText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text prayerText;
    [SerializeField] private TMP_Text prayerPotencyText;

    [SerializeField] private Health playerHealth;
    [SerializeField] private Magic playerMagic;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        ResolvePlayerReferences();
        RefreshRecordsUI();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResolvePlayerReferences();
        RefreshRecordsUI();
    }

    private void ResolvePlayerReferences()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            playerHealth = null;
            playerMagic = null;
            return;
        }

        playerHealth = player.GetComponentInChildren<Health>(true);
        playerMagic = player.GetComponentInChildren<Magic>(true);
    }
    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshRecordsUI();
        }
    }

    public void RefreshRecordsUI()
    {
        if (playtimeText != null && PlaytimeTracker.Instance != null)
        {
            playtimeText.text = "Playtime: " + PlaytimeTracker.Instance.GetFormattedPlaytime();
        }

        if (playerHealth != null && hpText != null)
        {
            hpText.text = $"HP: {playerHealth.health} / {playerHealth.maxHealth}";
        }

        if (ProgressionManager.Instance != null)
        {
            if (attackText != null)
                attackText.text = $"Attack: {ProgressionManager.Instance.attackPower}";

            if (playerMagic != null && prayerText != null)
            {
                int current = playerMagic.CurrentPrayers;
                int max = playerMagic.MaxPrayers;
                prayerText.text = $"Prayers: {current} / {max}";
            }
            else if (prayerText != null)
            {
                int maxPrayers = ProgressionManager.Instance.maxPrayers;
                prayerText.text = $"Prayers: {maxPrayers} / {maxPrayers}";
            }
            
            if (prayerPotencyText != null)
                prayerPotencyText.text = $"Prayer Potency: {ProgressionManager.Instance.prayerPotency}";
        }
    }
}
