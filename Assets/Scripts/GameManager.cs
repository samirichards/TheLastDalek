using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    
    public static bool IsGamePaused { get; set; } = false;
    [SerializeField] public static GameObject PauseMenu;
    [SerializeField] public static GameObject ItemUpgradeScreen;
    [SerializeField] public static GameObject ArtifactScreen;
    [SerializeField] public static AudioClip UpgradeAudioClip;
    private static bool IsOnNormalPauseScreen = false;
    [SerializeField] public AudioClip ArtifactScreenOpenClip;
    [SerializeField] public  AudioClip ArtifactScreenCloseClip;
    public static bool IsArtifactScreenShown = false;
    public static InventoryManager InventoryManagerComponent;

    [SerializeField] public static GameObject UpgradeScreenTitle;
    [SerializeField] public static GameObject UpgradeScreenIcon;
    private static InGameUI inGameUiRef;

    private static GameObject CameraObjectReference;

    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameManager();
                CameraObjectReference = GameObject.Find("CameraObject");
                _Instance = CameraObjectReference.GetComponentInChildren<GameManager>();
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(CameraObjectReference);
            }
            return _Instance;
        }
    }

    void Awake()
    {
        ResumeGame();
    }

    void Start()
    {
        InventoryManagerComponent = GetComponent<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!IsOnNormalPauseScreen)
            {
                if (IsGamePaused)
                {
                    HideArtifactScreen();
                }
                else
                {
                    ShowArtifactScreen();
                }
            }
        }
    }

    public static void ShowUpgradeScreen(int itemID, int itemTier)
    {
        Player.GetMovementController().StopSound();
        //CameraObjectReference.GetComponentInChildren<AudioSource>().PlayOneShot(UpgradeAudioClip);
        try
        {
            var item = InventoryManagerComponent.ItemDatabase.Items.First(a => a.ItemID == itemID);
            UpgradeScreenTitle.GetComponent<TextMeshProUGUI>().text = item.ItemName;
            //TODO Fix this so that it shows the correct item Tier (Should be an easy fix I just can't be bothered right now
            UpgradeScreenIcon.GetComponent<Image>().sprite = item.ItemTextures[itemTier];
            IsOnNormalPauseScreen = false;
            PauseMenu.SetActive(false);
            ItemUpgradeScreen.SetActive(true);
            ArtifactScreen.SetActive(false);
            IsGamePaused = true;
            Time.timeScale = 0f;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void ShowArtifactScreen()
    {
        try
        {
            GetComponent<Movement>().StopSound();
            //InventoryManagerComponent.DrawInventory();
            IsArtifactScreenShown = true;
            IsOnNormalPauseScreen = false;
            GetComponent<AudioSource>().PlayOneShot(ArtifactScreenOpenClip);
            PauseMenu.SetActive(false);
            ItemUpgradeScreen.SetActive(false);
            ArtifactScreen.SetActive(true);
            IsGamePaused = true;
            Time.timeScale = 0f;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void HideArtifactScreen()
    {
        IsArtifactScreenShown = false;
        GetComponent<AudioSource>().PlayOneShot(ArtifactScreenCloseClip);
        IsOnNormalPauseScreen = false;
        PauseMenu.SetActive(false);
        ItemUpgradeScreen.SetActive(false);
        ArtifactScreen.SetActive(false);
        IsGamePaused = false;
        Time.timeScale = 1.0f;
        GetComponent<InventoryManager>().UpdatePlayerAbilities();
    }

    public void ResumeGame()
    {
        IsOnNormalPauseScreen = false;
        PauseMenu.SetActive(false);
        IsGamePaused = false;
        Time.timeScale = 1.0f;
    }

    public void PauseGame()
    {
        GetComponent<Movement>().StopSound();
        IsOnNormalPauseScreen = true;
        PauseMenu.SetActive(true);
        IsGamePaused = true;
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
