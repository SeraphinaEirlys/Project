using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class SpellSlot : MonoBehaviour
{
    [Header("References")]
    public Image iconImage;
    public GameObject highlight;
    [SerializeField] private TMP_Text spellText;
    [SerializeField] private Image cooldownOverlay;

    public SpellSO AssignedSpell{get; private set;}

    [SerializeField] private Color normalColour;
    [SerializeField] private Color highlightColour = Color.white;
    private Vector3 normalScale = Vector3.one;
    private Vector3 highlightScale = Vector3.one * 1.2f;

    [Header("Pop Settings")]
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private float popDuration = .2f;


    public void SetSpell(SpellSO spellSO)
    {
        AssignedSpell = spellSO;

        if(spellSO != null)
        {
            cooldownOverlay.sprite = spellSO.icon;

            iconImage.sprite = spellSO.icon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            AssignedSpell = null;
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        cooldownOverlay.fillAmount = 0;

        SetHighlight(false);
    }



    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);

        iconImage.color = active ? highlightColour : normalColour;

        iconImage.rectTransform.localScale = normalScale;
        transform.localScale = active ? highlightScale : normalScale;

        if(active && AssignedSpell != null)
        {
            spellText.text = AssignedSpell.itemName;
        }

        spellText.enabled = active;
    }

    public void TriggerCooldown(float cooldownTime)
    {
        StartCoroutine(CooldownRoutine(cooldownTime));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        cooldownOverlay.fillAmount = 1;
        float elapsed = 0;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cooldownOverlay.fillAmount = 1 - (elapsed / duration);
            yield return null;
        }

        cooldownOverlay.fillAmount = 0;
        yield return StartCoroutine(PopEffect());
    }


    private IEnumerator PopEffect()
    {
        float elapsed = 0;
        float halfDuration = popDuration / 2;

        while(elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            iconImage.rectTransform.localScale = Vector3.Lerp(Vector3.one * popScale, normalScale, t);

            yield return null;
        }

        iconImage.rectTransform.localScale = normalScale;
    }
}
