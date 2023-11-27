using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class GameSettings
{
    /// <summary>
    /// Save an Options object to user file system
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static bool SaveSettings(Options options)
    {
        try
        {
            string json = JsonUtility.ToJson(options);
            Debug.Log(json);
            string jsonPath = Path.Combine(Application.persistentDataPath, "Options.json");
            System.IO.File.WriteAllText(jsonPath, json);
            OnOptionsChanged?.Invoke(options);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    /// <summary>
    /// Get an options object from user file system
    /// </summary>
    /// <returns></returns>
    public static Options GetSettings()
    {
        try
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, "Options.json")))
            {
                string rawJson = System.IO.File.ReadAllText(Path.Combine(Application.persistentDataPath, "Options.json"));
                Options options = JsonUtility.FromJson<Options>(rawJson);
                return options;
            }

            return new Options();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return new Options();
        }
    }

    public delegate void OptionsChanged(Options options);
    /// <summary>
    /// Subscribe to event to have values change as options are adjusted
    /// Must be unsubscribed from to prevent weird stuff happening
    /// </summary>
    public static event OptionsChanged OnOptionsChanged;
}
