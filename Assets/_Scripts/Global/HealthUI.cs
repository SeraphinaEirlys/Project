using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Health health;
    [SerializeField] private RectTransform healthBox;

    [Header("Health bar growth")]
    [SerializeField] private int baseMaxHealth = 70;
    [SerializeField] private float widthPerHealthPoint = 1.5f;

    private float baseWidth;
    private bool listeningToHealth;

    private void Awake()
    {
        if (healthBox != null)
            baseWidth = healthBox.rect.width;
    }

    private void OnEnable()
    {
        BindToHealth();
    }

    private void Start()
    {
        // Retry after scene objects have completed their initial activation.
        BindToHealth();
    }

    private void BindToHealth()
    {
        if (health == null)
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
                health = player.GetComponent<Health>();
        }

        if (health == null || fillImage == null || listeningToHealth)
            return;

        health.OnHealthChanged += UpdateHealthUI;
        listeningToHealth = true;
        UpdateHealthUI(health.health, health.maxHealth);
    }

    private void OnDisable()
    {
        if (health != null && listeningToHealth)
        {
            health.OnHealthChanged -= UpdateHealthUI;
            listeningToHealth = false;
        }
    }

    private void UpdateHealthUI(int currentHealth, int maximumHealth)
    {
        fillImage.fillAmount = maximumHealth > 0
            ? Mathf.Clamp01((float)currentHealth / maximumHealth)
            : 0f;

        if (healthBox != null)
        {
            int extraHealth = Mathf.Max(0, maximumHealth - baseMaxHealth);
            float targetWidth = baseWidth + extraHealth * widthPerHealthPoint;

            healthBox.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                targetWidth
            );
        }
    }
}
