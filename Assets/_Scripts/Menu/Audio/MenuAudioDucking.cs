using UnityEngine;

public class MenuAudioDucking : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float duckMultiplier = 0.4f;

    [SerializeField] private float fadeSpeed = 2f;

    private PauseMenu pauseMenu;
    private float normalListenerVolume;

    private void Awake()
    {
        pauseMenu = GetComponent<PauseMenu>();
        normalListenerVolume = AudioListener.volume;
    }

    private void Update()
    {
        bool menuIsOpen = pauseMenu != null &&
            (pauseMenu.isPaused ||
             (pauseMenu.inventoryMenu != null &&
              pauseMenu.inventoryMenu.IsOpen));

        bool shouldDuck = menuIsOpen || !Application.isFocused;

        float targetVolume = shouldDuck
            ? normalListenerVolume * duckMultiplier
            : normalListenerVolume;

        AudioListener.volume = Mathf.MoveTowards(
            AudioListener.volume,
            targetVolume,
            fadeSpeed * Time.unscaledDeltaTime);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            AudioListener.volume = normalListenerVolume * duckMultiplier;
    }

    private void OnDisable()
    {
        AudioListener.volume = normalListenerVolume;
    }
}