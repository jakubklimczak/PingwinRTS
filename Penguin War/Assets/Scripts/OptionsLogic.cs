using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsLogic : MonoBehaviour
{

    public AudioSource AudioMixer;
    public Dropdown ResolutionDropdown;
    public Dropdown QualityDropdown;
    public Dropdown TextureDropdown;
    public Dropdown AntiAliasingDropdown;
    public Slider VolumeSlider;
    public Toggle Fullscreen;
    float CurrentVolume;
    Resolution[] Resolutions;

    //This method sets the volume to a specified volume value (max volume -> 1.0, min volume -> 0.0)
    public void SetVolume(float Volume)
    {
        AudioMixer.volume = Volume;
        CurrentVolume = Volume;
    }


    // This method allows for switching on fullscreen mode (1=fullscreen enabled, 0=fullscreen disabled)
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    //This method uses a value passed in from the drop-down menu to set the resolution to. 
    public void SetResolution(int ResolutionIndex)
    {
        Resolution Res = Resolutions[ResolutionIndex];
        Screen.SetResolution(Res.width, Res.height, Screen.fullScreen);
    }

    // "6" is "Custom" quality and antialiasing option in Unity
    // These two methods allow for setting texture quality (texture size limit) and multi-sample anti aliasing to custom values
    public void SetTextureQuality(int TextureIndex)
    {
        QualitySettings.masterTextureLimit = TextureIndex;
        TextureDropdown.value = TextureIndex;
        QualityDropdown.value = 6;
    }
    public void SetAntiAliasing(int AAIndex)
    {
        QualitySettings.antiAliasing = AAIndex;
        AntiAliasingDropdown.value = AAIndex;
        QualityDropdown.value = 6;
    }

    public void SetQuality(int QualityIndex)
    {
        if (QualityIndex != 6) // if the user is not using any of the Unity presets
            QualitySettings.SetQualityLevel(QualityIndex);
        switch (QualityIndex)
        {
            case 0: // quality level - very low
                TextureDropdown.value = 0;
                AntiAliasingDropdown.value = 0;
                break;
            case 1: // quality level - low
                TextureDropdown.value = 1;
                AntiAliasingDropdown.value = 0;
                break;
            case 2: // quality level - medium
                TextureDropdown.value = 2;
                AntiAliasingDropdown.value = 0;
                break;
            case 3: // quality level - high
                TextureDropdown.value = 2;
                AntiAliasingDropdown.value = 1;
                break;
            case 4: // quality level - very high
                TextureDropdown.value = 2;
                AntiAliasingDropdown.value = 2;
                break;
            case 5: // quality level - ultra
                TextureDropdown.value = 2;
                AntiAliasingDropdown.value = 3;
                break;
        }

        QualityDropdown.value = QualityIndex;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("SavedQualitySetting", QualityDropdown.value);
        PlayerPrefs.SetInt("SavedResolution", ResolutionDropdown.value);
        PlayerPrefs.SetInt("SavedTextureQuality", TextureDropdown.value);
        PlayerPrefs.SetInt("SavedAntiAliasing", AntiAliasingDropdown.value);
        PlayerPrefs.SetInt("SavedFullscreen", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("SavedVolume", CurrentVolume);
    }

    public void SaveAndExit()
    {
        SaveSettings();
        ExitGame();
    }

    public void LoadSettings(int CurrentResolution)
    {
        //Quality
        if (PlayerPrefs.HasKey("SavedQualitySetting"))
        {
            QualityDropdown.value = PlayerPrefs.GetInt("SavedQualitySetting");
        }
        else
        {
            QualityDropdown.value = 3;

        }
        QualitySettings.SetQualityLevel(QualityDropdown.value);

        //Resolution
        if (PlayerPrefs.HasKey("SavedResolution"))
        {
            ResolutionDropdown.value = PlayerPrefs.GetInt("SavedResolution");
        }
        else
        {
            ResolutionDropdown.value = CurrentResolution;
        }
        SetResolution(ResolutionDropdown.value);

        //Texture quality
        if (PlayerPrefs.HasKey("SavedTextureQuality"))
        {
            TextureDropdown.value = PlayerPrefs.GetInt("SavedTextureQuality");
        }
        else
        {
            TextureDropdown.value = 3;
        }
        SetTextureQuality(TextureDropdown.value);


        //AA
        if (PlayerPrefs.HasKey("SavedAntiAliasing"))
        {
            AntiAliasingDropdown.value = PlayerPrefs.GetInt("SavedAntiAliasing");
        }
        else
        {
            AntiAliasingDropdown.value = 1;
        }
        SetAntiAliasing(AntiAliasingDropdown.value);

        //Fullscreen 
        if (PlayerPrefs.HasKey("SavedFullscreen"))
        {
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        }
        else
        {
            Screen.fullScreen = false;
        }

        //Volume
        if (PlayerPrefs.HasKey("SavedVolume"))
        {
            VolumeSlider.value = PlayerPrefs.GetFloat("SavedVolume");
        }
        else 
        {
        VolumeSlider.value = 0.50f;
        }
        SetVolume(VolumeSlider.value);
    }

    private void Start()
    {
        ResolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();
        Resolutions = Screen.resolutions;
        int CurrentResolution = 0;

        for (int i = 0; i < Resolutions.Length; i++)
        {
            string option = Resolutions[i].width + "x" + Resolutions[i].height + "  " + Resolutions[i].refreshRate + "Hz";
            resOptions.Add(option);
            if (Resolutions[i].width == Screen.width && Resolutions[i].height == Screen.height)
                CurrentResolution = i;
        }

        ResolutionDropdown.AddOptions(resOptions);
        ResolutionDropdown.RefreshShownValue();
        LoadSettings(CurrentResolution);
    }
}