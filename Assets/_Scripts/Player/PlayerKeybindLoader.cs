using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(PlayerInput))]
public class PlayerKeybindLoader : MonoBehaviour
{
    private void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();

        if (playerInput.actions == null)
            return;

        InputActionMap playerMap =
            playerInput.actions.FindActionMap("Player", false);

        GameplayKeybindProfile.LoadSavedOverrides(playerMap);
    }
}