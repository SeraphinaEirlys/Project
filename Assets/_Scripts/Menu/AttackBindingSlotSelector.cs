using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackBindingSlotSelector : MonoBehaviour
{
    [Header("Rebinding")]
    [SerializeField] private GameplayRebindController rebindController;
    [Header("Slot labels")]
    [SerializeField] private TMP_Text primaryText;
    [SerializeField] private TMP_Text secondaryText;
    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color dimColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private static AttackBindingSlotSelector activeSelector;
    private static int lastExitFrame = -1;
    public static bool BlocksMenuInput => activeSelector != null ||
        lastExitFrame == Time.frameCount || GameplayRebindController.BlocksMenuInput;

    private bool selecting;
    private int selectedSlot;
    private int openedFrame;
    private EventSystem lockedEventSystem;
    private bool previousNavigation;
    private GraphicRaycaster[] raycasters;
    private bool[] previousRaycasterStates;

    public void BeginSelection()
    {
        if (selecting || BlocksMenuInput) return;
        if (rebindController == null)
        {
            Debug.LogWarning("Attack: assign Rebind Controller in the Inspector.");
            return;
        }
        selecting = true;
        activeSelector = this;
        selectedSlot = 0;
        openedFrame = Time.frameCount;
        lockedEventSystem = EventSystem.current;
        if (lockedEventSystem != null)
        {
            lockedEventSystem.SetSelectedGameObject(gameObject);
            previousNavigation = lockedEventSystem.sendNavigationEvents;
            lockedEventSystem.sendNavigationEvents = false;
        }
        raycasters = GetComponentsInParent<GraphicRaycaster>(true);
        previousRaycasterStates = new bool[raycasters.Length];
        for (int i = 0; i < raycasters.Length; i++)
        {
            previousRaycasterStates[i] = raycasters[i].enabled;
            raycasters[i].enabled = false;
        }
        RefreshSlotColors();
    }

    private void Update()
    {
        if (!selecting || Time.frameCount == openedFrame) return;
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            EndSelection();
            return;
        }
        if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
        {
            selectedSlot = 0;
            RefreshSlotColors();
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
        {
            selectedSlot = 1;
            RefreshSlotColors();
        }
        if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
        {
            int slot = selectedSlot;
            EndSelection();
            rebindController.BeginAttackRebind(slot, gameObject);
        }
    }

    private void RefreshSlotColors()
    {
        if (primaryText != null) primaryText.color = selectedSlot == 0 ? selectedColor : dimColor;
        if (secondaryText != null) secondaryText.color = selectedSlot == 1 ? selectedColor : dimColor;
    }

    private void EndSelection()
    {
        if (!selecting) return;
        selecting = false;
        lastExitFrame = Time.frameCount;
        if (activeSelector == this) activeSelector = null;
        if (lockedEventSystem != null) lockedEventSystem.sendNavigationEvents = previousNavigation;
        if (raycasters != null)
            for (int i = 0; i < raycasters.Length; i++)
                if (raycasters[i] != null) raycasters[i].enabled = previousRaycasterStates[i];
        if (primaryText != null) primaryText.color = normalColor;
        if (secondaryText != null) secondaryText.color = normalColor;
    }

    private void OnDisable() => EndSelection();
}
