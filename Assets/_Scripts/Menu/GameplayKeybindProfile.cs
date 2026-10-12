using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameplayKeybindProfile : MonoBehaviour
{
    public const string SaveKey = "GameplayKeybinds_v1";

    [SerializeField] private InputActionAsset sourceActions;

    private InputActionAsset runtimeActions;

    public InputActionMap PlayerMap
    {
        get
        {
            EnsureInitialized();

            return runtimeActions != null
                ? runtimeActions.FindActionMap("Player", true)
                : null;
        }
    }

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnEnable()
    {
        InputActionMap map = PlayerMap;

        if (map != null)
            LoadSavedOverrides(map);
    }

    private void EnsureInitialized()
    {
        if (runtimeActions != null || sourceActions == null)
            return;

        // Work on a runtime copy, not the project asset.
        runtimeActions = Instantiate(sourceActions);
        runtimeActions.Disable();
    }

    public void SaveAndApply()
    {
        InputActionMap map = PlayerMap;

        if (map == null)
            return;

        string json = map.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        // Update players already present in the running game.
        PlayerInput[] players = FindObjectsByType<PlayerInput>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (PlayerInput player in players)
        {
            if (player.actions == null)
                continue;

            InputActionMap liveMap =
                player.actions.FindActionMap("Player", false);

            if (liveMap != null)
                LoadSavedOverrides(liveMap);
        }
    }

    public static void LoadSavedOverrides(InputActionMap map)
    {
        if (map == null || !PlayerPrefs.HasKey(SaveKey))
            return;

        string json = PlayerPrefs.GetString(SaveKey);

        if (string.IsNullOrEmpty(json))
            return;

        // Preserve which actions were enabled.
        List<InputAction> enabledActions = new List<InputAction>();

        foreach (InputAction action in map.actions)
        {
            if (action.enabled)
                enabledActions.Add(action);
        }

        map.Disable();

        try
        {
            map.LoadBindingOverridesFromJson(json);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning(
                "Could not load gameplay keybinds: " +
                exception.Message);
        }
        finally
        {
            foreach (InputAction action in enabledActions)
                action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (runtimeActions != null)
            Destroy(runtimeActions);
    }
}