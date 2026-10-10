using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action<Vector2, float> OnDamaged;
    public event Action<Vector2, float> OnDeath;

    public int health;
    public int maxHealth;

    public bool isInvincible { get; set; } = false;

    [Header("UI Popups")]
    public GameObject damagePopupPrefab;

    private void Start()
    {
        health = maxHealth;
    }

    public void ChangeHealth(int amount, Vector2 sourcePosition, float knockbackForce = 0f)
    {
        if (isInvincible && amount < 0) return;

        health += amount;

        bool showPopup = PlayerPrefs.GetInt("ShowDamagePopup", 1) == 1;

        if (showPopup && amount < 0 && damagePopupPrefab != null)
        {
            float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
            Vector3 spawnPos = transform.position + new Vector3(randomX, 0.5f, 0f);
            
            GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup(amount);
            }
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            OnDeath?.Invoke(sourcePosition, knockbackForce);
        }
        else if (amount < 0)
        {
            OnDamaged?.Invoke(sourcePosition, knockbackForce);
        }
    }
}