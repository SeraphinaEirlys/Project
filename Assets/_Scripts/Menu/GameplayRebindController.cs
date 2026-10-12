using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class GameplayRebindController : MonoBehaviour
{
    [Serializable]
    public class BindingLabel
    {
        public string displayName;
        public string actionName;
        public string originalPath;
        public TMP_Text text;
    }

    [SerializeField] private GameplayKeybindProfile profile;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private CanvasGroup statusGroup;
    [SerializeField] private float resultVisibleSeconds = 2.5f;
    [SerializeField] private float statusFadeSeconds = 0.25f;
    [SerializeField] private BindingLabel[] labels;

    private static GameplayRebindController active;
    private static int exitFrame = -1;
    public static bool BlocksMenuInput => active != null || exitFrame == Time.frameCount;

    private InputActionRebindingExtensions.RebindingOperation operation;
    private Coroutine routine;
    private EventSystem eventSystem;
    private bool previousNavigation;
    private GraphicRaycaster[] raycasters;
    private bool[] raycasterStates;
    private readonly List<InputAction> enabledActions = new List<InputAction>();
    private GameObject returnSelection;
    private float statusTimer = -1f;
    private bool fadingStatus;

    private void Start() => RefreshLabels();

    private void OnEnable() => HideStatus();

    private void Update()
    {
        if (statusTimer >= 0f)
        {
            statusTimer -= Time.unscaledDeltaTime;
            if (statusTimer <= 0f)
            {
                statusTimer = -1f;
                fadingStatus = true;
            }
        }
        if (!fadingStatus) return;
        if (statusGroup == null || statusFadeSeconds <= 0f)
        {
            HideStatus();
            return;
        }
        statusGroup.alpha = Mathf.MoveTowards(statusGroup.alpha, 0f,
            Time.unscaledDeltaTime / statusFadeSeconds);
        if (statusGroup.alpha <= 0f) HideStatus();
    }

    private void HideStatus()
    {
        statusTimer = -1f;
        fadingStatus = false;
        if (statusText != null) statusText.text = "";
        if (statusGroup != null)
        {
            statusGroup.alpha = 0f;
            statusGroup.interactable = false;
            statusGroup.blocksRaycasts = false;
        }
    }

    public void BeginAttackRebind(int slot, GameObject selection)
    {
        BeginRebind("Attack", slot == 0 ? "<Keyboard>/j" : "<Mouse>/leftButton", selection);
    }

    public void BeginRebind(string actionName, string originalPath, GameObject selection)
    {
        if (!isActiveAndEnabled || BlocksMenuInput)
            return;

        InputActionMap map = profile != null ? profile.PlayerMap : null;
        InputAction action = map != null ? map.FindAction(actionName, false) : null;
        int index = FindBinding(action, originalPath);
        if (index < 0)
        {
            Report("Binding not found: " + actionName + " / " + originalPath);
            return;
        }

        active = this;
        returnSelection = selection;
        LockInput();
        routine = StartCoroutine(Capture(action, index));
    }

    private IEnumerator Capture(InputAction action, int index)
    {
        Report("Release the confirm button to begin...", true);
        yield return null;
        while (ConfirmHeld())
            yield return null;

        bool finished = false;
        bool cancelled = false;
        string newPath = null;
        ButtonControl capturedButton = null;
        Report("Press a key or mouse button. Esc to cancel.", true);

        try
        {
            operation = action.PerformInteractiveRebinding(index)
                .WithExpectedControlType("Button")
                .WithControlsHavingToMatchPath("<Keyboard>")
                .WithControlsHavingToMatchPath("<Mouse>")
                .WithControlsExcluding("<Keyboard>/anyKey")
                .WithCancelingThrough("<Keyboard>/escape")
                // Intercept: validate collisions before changing any binding.
                .OnApplyBinding((op, path) => newPath = path)
                .OnComplete(op =>
                {
                    capturedButton = op.selectedControl as ButtonControl;
                    finished = true;
                })
                .OnCancel(op => { cancelled = true; finished = true; });
            operation.Start();
        }
        catch (Exception exception)
        {
            Report("Could not start rebinding: " + exception.Message);
            finished = true;
            cancelled = true;
        }

        while (!finished)
            yield return null;

        if (operation != null)
        {
            operation.Dispose();
            operation = null;
        }

        if (cancelled)
            Report("Rebinding cancelled.");
        else
            ApplyValidated(action, index, newPath);

        // Do not let the captured key activate menus or gameplay on release.
        while ((capturedButton != null && capturedButton.isPressed) ||
               (Keyboard.current != null && Keyboard.current.escapeKey.isPressed))
            yield return null;
        yield return null;
        UnlockInput();
        routine = null;
    }

    private void ApplyValidated(InputAction target, int index, string newPath)
    {
        if (string.IsNullOrEmpty(newPath))
        {
            Report("No new input detected.");
            return;
        }

        string oldPath = target.bindings[index].effectivePath;
        if (SamePath(oldPath, newPath))
        {
            Report("This input is already assigned to this binding.");
            return;
        }

        InputAction duplicateAction = null;
        int duplicateIndex = -1;
        int count = 0;
        foreach (InputAction action in profile.PlayerMap.actions)
        {
            if (!IsUsedAction(action.name)) continue;
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action == target && i == index) continue;
                if (action.bindings[i].isComposite) continue;
                if (!SamePath(action.bindings[i].effectivePath, newPath)) continue;
                duplicateAction = action;
                duplicateIndex = i;
                count++;
            }
        }

        BindingLabel duplicateLabel = count == 1
            ? FindLabel(duplicateAction, duplicateIndex) : null;
        if (count > 1 || (count == 1 && duplicateLabel == null))
        {
            Report("This input conflicts with hidden or multiple bindings. Choose another input.");
            return;
        }
        if (count == 1 && string.IsNullOrEmpty(oldPath))
        {
            Report("This slot is empty. There is no previous input to swap.");
            return;
        }

        string backup = profile.PlayerMap.SaveBindingOverridesAsJson();
        try
        {
            target.ApplyBindingOverride(index, newPath);
            if (count == 1)
                duplicateAction.ApplyBindingOverride(duplicateIndex, oldPath);
            profile.SaveAndApply();
            RefreshLabels();
            string targetName = LabelName(target, index);
            Report(count == 1
                ? "Swapped bindings: " + targetName + " and " + duplicateLabel.displayName + "."
                : targetName + " bound to " + Display(target, index) + ".");
        }
        catch (Exception exception)
        {
            profile.PlayerMap.LoadBindingOverridesFromJson(backup);
            profile.SaveAndApply();
            RefreshLabels();
            Report("Could not change binding: " + exception.Message);
        }
    }

    public void RefreshLabels()
    {
        if (profile == null || profile.PlayerMap == null || labels == null) return;
        foreach (BindingLabel label in labels)
        {
            if (label == null) continue;
            InputAction action = profile.PlayerMap.FindAction(label.actionName, false);
            int index = FindBinding(action, label.originalPath);
            if (index >= 0 && label.text != null)
                label.text.text = Display(action, index);
        }
    }

    private static string Display(InputAction action, int index)
    {
        string path = action.bindings[index].effectivePath;
        if (SamePath(path, "<Mouse>/leftButton")) return "LMB";
        if (SamePath(path, "<Mouse>/rightButton")) return "RMB";
        if (SamePath(path, "<Mouse>/middleButton")) return "MMB";
        return action.GetBindingDisplayString(index,
            InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    private static int FindBinding(InputAction action, string originalPath)
    {
        if (action == null) return -1;
        for (int i = 0; i < action.bindings.Count; i++)
            if (!action.bindings[i].isComposite && SamePath(action.bindings[i].path, originalPath))
                return i;
        return -1;
    }

    private BindingLabel FindLabel(InputAction action, int index)
    {
        if (labels == null) return null;
        foreach (BindingLabel label in labels)
            if (label != null && label.actionName == action.name &&
                SamePath(label.originalPath, action.bindings[index].path)) return label;
        return null;
    }

    private string LabelName(InputAction action, int index)
    {
        BindingLabel label = FindLabel(action, index);
        return label != null ? label.displayName : action.name;
    }

    private static bool SamePath(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static bool IsUsedAction(string name) => name == "Move" || name == "Jump" ||
        name == "Run" || name == "Attack" || name == "Dash" ||
        name == "Interact" || name == "Spellcast";

    private static bool ConfirmHeld() =>
        (Keyboard.current != null && (Keyboard.current.enterKey.isPressed ||
            Keyboard.current.numpadEnterKey.isPressed || Keyboard.current.spaceKey.isPressed)) ||
        (Mouse.current != null && Mouse.current.leftButton.isPressed);

    private void LockInput()
    {
        eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            previousNavigation = eventSystem.sendNavigationEvents;
            eventSystem.sendNavigationEvents = false;
        }
        raycasters = GetComponentsInParent<GraphicRaycaster>(true);
        raycasterStates = new bool[raycasters.Length];
        for (int i = 0; i < raycasters.Length; i++)
        {
            raycasterStates[i] = raycasters[i].enabled;
            raycasters[i].enabled = false;
        }
        enabledActions.Clear();
        foreach (PlayerInput player in FindObjectsByType<PlayerInput>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            InputActionMap map = player.actions != null
                ? player.actions.FindActionMap("Player", false) : null;
            if (map == null) continue;
            foreach (InputAction action in map.actions)
                if (action.enabled && !enabledActions.Contains(action)) enabledActions.Add(action);
            map.Disable();
        }
    }

    private void UnlockInput()
    {
        if (active != this) return;
        if (eventSystem != null)
        {
            eventSystem.sendNavigationEvents = previousNavigation;
            if (returnSelection != null && returnSelection.activeInHierarchy)
                eventSystem.SetSelectedGameObject(returnSelection);
        }
        if (raycasters != null)
            for (int i = 0; i < raycasters.Length; i++)
                if (raycasters[i] != null) raycasters[i].enabled = raycasterStates[i];
        foreach (InputAction action in enabledActions) action.Enable();
        enabledActions.Clear();
        active = null;
        exitFrame = Time.frameCount;
    }

    private void Report(string message, bool keepVisible = false)
    {
        if (statusText != null) statusText.text = message;
        if (statusGroup != null)
        {
            statusGroup.alpha = 1f;
            statusGroup.interactable = false;
            statusGroup.blocksRaycasts = false;
        }
        fadingStatus = false;
        statusTimer = keepVisible ? -1f : Mathf.Max(0.1f, resultVisibleSeconds);
        Debug.Log("Keybind: " + message);
    }

    private void OnDisable()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
        if (operation != null) operation.Dispose();
        operation = null;
        UnlockInput();
        HideStatus();
    }
}
