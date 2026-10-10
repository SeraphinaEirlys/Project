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

    [Header("Graphics - Screen Mode")]
    public TMP_Dropdown screenModeDropdown;

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

    void InitScreenModes()
    {
        if (screenModeDropdown != null)
        {
            screenModeDropdown.ClearOptions();
            List<string> modes = new List<string> { "Fullscreen", "Borderless Window", "Windowed" };
            screenModeDropdown.AddOptions(modes);
            
            screenModeDropdown.value = PlayerPrefs.GetInt("ScreenMode", 0);
            screenModeDropdown.RefreshShownValue();
        }
    }

    public void SetScreenMode(int index)
    {
        switch (index)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }
        PlayerPrefs.SetInt("ScreenMode", index);
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
}