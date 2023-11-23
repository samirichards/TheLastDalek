using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; } = false;
    public static bool IsOnNormalPauseScreen = false;
    public static bool IsArtifactScreenShown = false;
    private static InGameUI inGameUiRef;
    private static GameObject CameraObjectReference;
    public static AudioSource _audioSource;
    private static int _TotalExterminations = 0;
    [SerializeField] private ItemDatabaseObject itemDatabase;
    private static ItemDatabaseObject _itemDatabase;
    private static LevelTransitionManager _levelTransitionManager;

    private static GameObject _UpgradeScreenTitle;
    private static GameObject _UpgradeScreenIcon;
    private static GameObject _PauseMenu;
    private static GameObject _ItemUpgradeScreen;
    private static GameObject _ArtifactScreen;
    private static TextMeshProUGUI _TotalExterminationsCount;

    [SerializeField] public GameObject UpgradeScreenTitle;
    [SerializeField] public GameObject UpgradeScreenIcon;
    [SerializeField] public GameObject PauseMenu;
    [SerializeField] public GameObject ItemUpgradeScreen;
    [SerializeField] public GameObject ArtifactScreen;
    [SerializeField] public GameObject TotalExterminationsCount;


    private static AudioClip _ArtifactScreenOpenClip;
    private static AudioClip _ArtifactScreenCloseClip;
    private static AudioClip _UpgradeAudioClip;

    [SerializeField] public AudioClip ArtifactScreenOpenClip;
    [SerializeField] public AudioClip ArtifactScreenCloseClip;
    [SerializeField] public AudioClip UpgradeAudioClip;

    private static GameManager _GameManagerInstance;

    public static GameManager Instance
    {
        get
        {
            if (!_GameManagerInstance)
            {
                _GameManagerInstance = new GameManager();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_GameManagerInstance);
            }
            return _GameManagerInstance;
        }
    }

    void Awake()
    {

        if (_GameManagerInstance == null)
        {
            DontDestroyOnLoad(gameObject);
            _GameManagerInstance = this;
            _levelTransitionManager = GetComponent<LevelTransitionManager>();
            _audioSource = GetComponent<AudioSource>();
            _UpgradeAudioClip = UpgradeAudioClip;
            _ArtifactScreenCloseClip = ArtifactScreenCloseClip;
            _ArtifactScreenOpenClip = ArtifactScreenOpenClip;

            _UpgradeScreenTitle = UpgradeScreenTitle;
            _UpgradeScreenIcon = UpgradeScreenIcon;
            _PauseMenu = PauseMenu;
            _ItemUpgradeScreen = ItemUpgradeScreen;
            _ArtifactScreen = ArtifactScreen;
            _TotalExterminationsCount = TotalExterminationsCount.GetComponent<TextMeshProUGUI>();
            _itemDatabase = itemDatabase;
        }
        else if (_GameManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    static GameManager()
    {
        //ResumeGame();
    }

    /*
     Change this so that another script checks for which keys are being pressed, and then calls pause game and all that fun stuff in here
    void Update()
    {

    }
    */

    public static void ShowUpgradeScreen(int itemID, int itemTier)
    {
        Player._PropController.StopSound();
        try
        {
            IsOnNormalPauseScreen = false;
            IsGamePaused = true;
            _audioSource.PlayOneShot(_UpgradeAudioClip);
            _ItemUpgradeScreen.SetActive(true);
            _UpgradeScreenTitle.GetComponent<TextMeshProUGUI>().text =
                _itemDatabase.Items.First(a => a.ItemID == itemID).ItemName;
            _UpgradeScreenIcon.GetComponent<Image>().sprite = 
                _itemDatabase.Items.First(a=>a.ItemID == itemID).ItemTextures[itemTier];
            Time.timeScale = 0f;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static ItemDatabaseObject GetItemDatabase()
    {
        return _itemDatabase;
    }

    public static LevelTransitionManager GetLevelTransitionManager()
    {
        return _levelTransitionManager;
    }

    public static void ShowArtifactScreen()
    {
        _ItemUpgradeScreen.SetActive(false);
        Player._PropController.StopSound();
        IsArtifactScreenShown = true;
        IsOnNormalPauseScreen = false;
        IsGamePaused = true;
        _audioSource.PlayOneShot(_ArtifactScreenOpenClip);
        _ArtifactScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public static void HideArtifactScreen()
    {
        IsArtifactScreenShown = false;
        _audioSource.PlayOneShot(_ArtifactScreenCloseClip);
        IsOnNormalPauseScreen = false;
        IsGamePaused = false;
        Time.timeScale = 1.0f;
        _ArtifactScreen.SetActive(false);
        _ItemUpgradeScreen.SetActive(false);
        Player._inventoryController.RunUpdateAbilities();
    }

    public static void ResumeGame()
    {
        IsOnNormalPauseScreen = false;
        _PauseMenu.SetActive(false);
        IsGamePaused = false;
        Time.timeScale = 1.0f;
    }

    public static void PauseGame()
    {
        Player._PropController.StopSound();
        IsOnNormalPauseScreen = true;
        _PauseMenu.SetActive(true);
        IsGamePaused = true;
        Time.timeScale = 0f;
        _TotalExterminationsCount.text = _TotalExterminations.ToString();
    }

    public static void IncrementExterminations()
    {
        _TotalExterminations++;
    }

    public static int GetTotalExterminations()
    {
        return _TotalExterminations;
    }

    public static void QuitToMainMenu()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(GameObject.FindGameObjectWithTag("CameraObject"));
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public static void QuitGame()
    {
        Application.Quit(0);
    }
}
