using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    
    public bool IsGamePaused { get; set; } = false;
    [SerializeField] public GameObject PauseMenu;
    [SerializeField] public GameObject ItemUpgradeScreen;
    [SerializeField] public GameObject ArtifactScreen;
    [SerializeField] public AudioClip UpgradeAudioClip;
    private bool IsOnNormalPauseScreen = false;
    [SerializeField] public AudioClip ArtifactScreenOpenClip;
    [SerializeField] public AudioClip ArtifactScreenCloseClip;
    public bool IsArtifactScreenShown = false;
    public InventoryManager InventoryManagerComponent;

    [SerializeField] public GameObject UpgradeScreenTitle;
    [SerializeField] public GameObject UpgradeScreenIcon;
    private InGameUI inGameUiRef;

    private GameObject CameraObjectReference;

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
            Debug.Log("ESC Key pressed");
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
            //TODO Fix this
            Debug.Log("TAB Pressed");
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

    public void ShowUpgradeScreen(int itemID, int itemTier)
    {
        Debug.Log("Upgrade Screen show: " + InventoryManagerComponent.ItemDatabase.Items.First(a=>a.ItemID == itemID).ItemTitle);
        Player.GetMovementController().StopSound();
        try
        {
            IsOnNormalPauseScreen = false;
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
            IsArtifactScreenShown = true;
            IsOnNormalPauseScreen = false;
            GetComponent<AudioSource>().PlayOneShot(ArtifactScreenOpenClip);
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
        IsGamePaused = false;
        Time.timeScale = 1.0f;
        GetComponent<InventoryManager>().UpdatePlayerAbilities();
    }

    public void ResumeGame()
    {
        IsOnNormalPauseScreen = false;
        //PauseMenu.SetActive(false);
        IsGamePaused = false;
        Time.timeScale = 1.0f;
    }

    public void PauseGame()
    {
        GetComponent<Movement>().StopSound();
        IsOnNormalPauseScreen = true;
        //PauseMenu.SetActive(true);
        IsGamePaused = true;
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
