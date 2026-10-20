using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrayerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Magic magic;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Image iconPrototype;

    [Header("Icon appearance")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color usedColor = new Color(1f, 1f, 1f, 0.25f);

    private readonly List<Image> clonedIcons = new();

    private void OnEnable()
    {
        if (magic == null)
            magic = FindFirstObjectByType<Magic>();

        if (magic != null)
            magic.OnPrayersChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (magic != null)
            magic.OnPrayersChanged -= Refresh;
    }

    private void Refresh()
    {
        if (magic == null || iconContainer == null || iconPrototype == null)
            return;

        int maxPrayers = Mathf.Max(0, magic.MaxPrayers);
        int currentPrayers = Mathf.Clamp(magic.CurrentPrayers, 0, maxPrayers);
        int clonesNeeded = Mathf.Max(0, maxPrayers - 1);

        if (maxPrayers == 0)
        {
            iconPrototype.gameObject.SetActive(false);
        }
        else
        {
            iconPrototype.gameObject.SetActive(true);
        }

        while (clonedIcons.Count < clonesNeeded)
        {
            Image icon = Instantiate(iconPrototype, iconContainer);
            clonedIcons.Add(icon);
        }

        while (clonedIcons.Count > clonesNeeded)
        {
            int lastIndex = clonedIcons.Count - 1;
            Destroy(clonedIcons[lastIndex].gameObject);
            clonedIcons.RemoveAt(lastIndex);
        }

        for (int i = 0; i < maxPrayers; i++)
        {
            Image icon = i == 0 ? iconPrototype : clonedIcons[i - 1];
            icon.color = i < currentPrayers
                ? availableColor
                : usedColor;
        }
    }
}
