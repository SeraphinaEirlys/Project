using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Game";

    [Header("UI")]
    public GameObject optionPanel;

    [Header("Audio Setup")]
    public AudioMixer audioMixer;

    private void Start()
    {
        if (optionPanel != null) optionPanel.SetActive(false);

        LoadVolumeAtStart();

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic("MainMenuTheme", 1f);
        }
    }

    private void Update()
    {
        if (optionPanel != null && optionPanel.activeSelf &&
            Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseOptionPanel();
        }
    }

    private void LoadVolumeAtStart()
    {
        if (audioMixer == null) return;

        float musicVal = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVal = PlayerPrefs.GetFloat("SFXVolume", 1f);

        float musicDb = (musicVal <= 0.0001f) ? -80f : Mathf.Log10(musicVal) * 40f;
        float sfxDb = (sfxVal <= 0.0001f) ? -80f : Mathf.Log10(sfxVal) * 20f;

        audioMixer.SetFloat("MusicVolume", musicDb);
        audioMixer.SetFloat("SFXVolume", sfxDb);
    }

    public void OpenOptionPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(true);
    }

    public void CloseOptionPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadScene(gameSceneName, "CrossFade");
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}