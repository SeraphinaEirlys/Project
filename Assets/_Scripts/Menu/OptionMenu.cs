using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Pages (0:Gameplay, 1:Sound, 2:Graphics, 3:Control)")]
    public GameObject[] optionPages; 
    private int currentPageIndex = 0;

    [Header("Gameplay")]
    public TMP_Text damagePopupText;
    public TMP_Text recoveryPopupText;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Graphics - Display Mode")]
    public TMP_Text screenModeText;

    private int currentScreenMode;

    private readonly string[] screenModeNames =
    {
        "Fullscreen",
        "Windowed Borderless",
        "Windowed"
    };


    [Header("Tab Navigation")]
    public TabGroup settingsTabGroup;
    

    private void OnEnable() => ApplySavedSettings();

    private void Update()
    {
        if (settingsTabGroup == null)
            return;

        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.qKey.wasPressedThisFrame)
                settingsTabGroup.Prev();
            else if (keyboard.eKey.wasPressedThisFrame)
                settingsTabGroup.Next();
        }

        if (currentPageIndex != settingsTabGroup.CurrentIndex)
            SwitchPage(settingsTabGroup.CurrentIndex);
    }

    public void ApplySavedSettings()
    {
        InitScreenModes();
        LoadSettings();
        SwitchPage(0);
    }

    public void SwitchPage(int pageIndex)
    {
        if (optionPages == null || optionPages.Length == 0)
            return;

        currentPageIndex =
            (pageIndex % optionPages.Length + optionPages.Length)
            % optionPages.Length;

        for (int i = 0; i < optionPages.Length; i++)
        {
            if (optionPages[i] != null)
                optionPages[i].SetActive(i == currentPageIndex);
        }

        if (settingsTabGroup != null)
            settingsTabGroup.Select(currentPageIndex);
    }

    public void NextPage()
    {
        if (optionPages.Length == 0) return;
        int next = (currentPageIndex + 1) % optionPages.Length;
        SwitchPage(next);
    }

    public void PreviousPage()
    {
        if (optionPages.Length == 0) return;
        int prev = (currentPageIndex - 1 + optionPages.Length) % optionPages.Length;
        SwitchPage(prev);
    }

    public void ToggleDamagePopup()
    {
        bool isOn = PlayerPrefs.GetInt("ShowDamagePopup", 1) == 1;
        
        isOn = !isOn; 

        PlayerPrefs.SetInt("ShowDamagePopup", isOn ? 1 : 0);

        if (damagePopupText != null)
        {
            damagePopupText.text = isOn ? "On" : "Off";
        }
    }

    public void ToggleRecoveryPopup()
    {
        bool isOn = PlayerPrefs.GetInt("ShowRecoveryPopup", 1) == 1;
        isOn = !isOn;

        PlayerPrefs.SetInt("ShowRecoveryPopup", isOn ? 1 : 0);
        PlayerPrefs.Save();

        if (recoveryPopupText != null)
            recoveryPopupText.text = isOn ? "On" : "Off";
    }

    private void InitScreenModes()
    {
        int savedMode = PlayerPrefs.GetInt("ScreenMode", 0);
        SetScreenMode(savedMode);
    }

    public void CycleScreenMode()
    {
        int nextMode = (currentScreenMode + 1) % screenModeNames.Length;
        SetScreenMode(nextMode);

        PlayerPrefs.Save();
    }

    public void SetScreenMode(int index)
    {
        currentScreenMode = Mathf.Clamp(
            index, 0, screenModeNames.Length - 1);

        switch (currentScreenMode)
        {
            case 0:
                Screen.fullScreenMode =
                    FullScreenMode.ExclusiveFullScreen;
                break;

            case 1:
                Screen.fullScreenMode =
                    FullScreenMode.FullScreenWindow;
                break;

            case 2:
                Screen.fullScreenMode =
                    FullScreenMode.Windowed;
                break;
        }

        if (screenModeText != null)
            screenModeText.text = screenModeNames[currentScreenMode];

        PlayerPrefs.SetInt("ScreenMode", currentScreenMode);
    }

    public void SetMusicVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f)
            audioMixer.SetFloat("MusicVolume", -80f);
        else
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 40f);
        
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f)
            audioMixer.SetFloat("SFXVolume", -80f);
        else
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20f);
        
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    void LoadSettings()
    {
        bool isPopupOn = PlayerPrefs.GetInt("ShowDamagePopup", 1) == 1; 
        bool showRecovery = PlayerPrefs.GetInt("ShowRecoveryPopup", 1) == 1;
        if (damagePopupText != null)
        {
            damagePopupText.text = isPopupOn ? "On" : "Off";
        }

        if (recoveryPopupText != null)
            recoveryPopupText.text = showRecovery ? "On" : "Off";

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVal = PlayerPrefs.GetFloat("MusicVolume");
            if (musicSlider != null) musicSlider.value = musicVal;
            SetMusicVolume(musicVal);
        }
        else if (musicSlider != null) musicSlider.value = 1f;

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVal = PlayerPrefs.GetFloat("SFXVolume");
            if (sfxSlider != null) sfxSlider.value = sfxVal;
            SetSFXVolume(sfxVal);
        }
        else if (sfxSlider != null) sfxSlider.value = 1f;
    }

    public void RevertCurrentTabToDefault()
    {
        // Read the visible tab directly to avoid a one-frame delay.
        int tabIndex = settingsTabGroup != null
            ? settingsTabGroup.CurrentIndex
            : currentPageIndex;

        switch (tabIndex)
        {
            case 0: // Gameplay
                PlayerPrefs.SetInt("ShowDamagePopup", 1);
                PlayerPrefs.SetInt("ShowRecoveryPopup", 1);

                if (damagePopupText != null)
                    damagePopupText.text = "On";

                if (recoveryPopupText != null)
                    recoveryPopupText.text = "On";

                break;

            case 1: // Sound
                // Update UI without triggering slider callbacks twice.
                if (musicSlider != null)
                    musicSlider.SetValueWithoutNotify(1f);

                if (sfxSlider != null)
                    sfxSlider.SetValueWithoutNotify(1f);

                if (audioMixer != null)
                {
                    SetMusicVolume(1f);
                    SetSFXVolume(1f);
                }
                else
                {
                    PlayerPrefs.SetFloat("MusicVolume", 1f);
                    PlayerPrefs.SetFloat("SFXVolume", 1f);
                    Debug.LogWarning(
                        "OptionsMenu: AudioMixer has not been assigned.",
                        this);
                }

                break;

            case 2: // Graphics
                SetScreenMode(0);
                break;

            case 3: // Control
                // Add reset logic when control settings are implemented.
                break;
        }

        PlayerPrefs.Save();
    }
}