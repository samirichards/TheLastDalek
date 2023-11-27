using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Options
{
    [Serializable]
    public enum GraphicsSettings
    {
        Low,
        Medium,
        High
    }

    [Serializable]
    public enum LighingSettings
    {
        Low,
        Medium,
        High
    }

    [SerializeField] public float SFXVolume;
    [SerializeField] public float MusicVolume;
    [SerializeField] public int graphicsSettings;
    [SerializeField] public int lighingSettings;

    public Options()
    {
        SFXVolume = 1.0f;
        MusicVolume = 1.0f;
        graphicsSettings = (int)GraphicsSettings.High;
        lighingSettings = (int)LighingSettings.High;
    }

    public GraphicsSettings GetGraphicsSettingsEnum()
    {
        return (GraphicsSettings)graphicsSettings;
    }

    public LighingSettings GetLighingSettingsEnum()
    {
        return (LighingSettings)lighingSettings;
    }

    public void Save()
    {
        GameSettings.SaveSettings(this);
    }
}
