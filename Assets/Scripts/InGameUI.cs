using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InGameUI : MonoBehaviour
{
    private static InGameUI _inGameUiInstance;
    private static GameObject UICanvasRef;

    [SerializeField] public static GameObject PauseMenu;
    [SerializeField] public static GameObject ItemUpgradeScreen;
    [SerializeField] public static GameObject ArtifactScreen;

    [SerializeField] public Slider HealthBar;
    [SerializeField] public Slider ShieldHealth;
    [SerializeField] public GameObject ShieldHealthContainer;

    [SerializeField] private Button UpgradeToArtifactButton;

    [SerializeField] private GameObject ItemSelectionSlotA;
    [SerializeField] private GameObject ItemSelectionSlotB;

    [SerializeField] private AudioClip ItemHoverSound;
    [SerializeField] private AudioClip ItemSelectSound;

    // Start is called before the first frame update

    public static InGameUI Instance
    {
        get
        {
            if (!_inGameUiInstance)
            {
                _inGameUiInstance = new InGameUI();
                UICanvasRef = GameObject.FindWithTag("InGameUI");
                _inGameUiInstance = UICanvasRef.GetComponent<InGameUI>();
                _inGameUiInstance.name = _inGameUiInstance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(UICanvasRef);
            }
            return _inGameUiInstance;
        }
    }

    public static GameObject GetUICanvasRef()
    {
        return UICanvasRef;
    }

    public void GoToArtifactScreen()
    {
        GameManager.ShowArtifactScreen();
        GetComponentInChildren<ArtifactScreen>().PopulateSlots();
        UpdateSelectedItems();
    }

    void Awake()
    {
        UpgradeToArtifactButton.onClick.AddListener(()=>{GoToArtifactScreen();});
        UICanvasRef = GameObject.FindWithTag("InGameUI");
        //PauseMenu = GameObject.FindWithTag("PauseMenu");
        //ItemUpgradeScreen = GameObject.FindWithTag("ItemUpgradeScreen");
        //ArtifactScreen = GameObject.FindWithTag("ArtifactScreen");
        DontDestroyOnLoad(UICanvasRef);

    }

    void Start()
    {
        HealthBar.maxValue = Player._playerComponent.MaxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC Key pressed");
            if (GameManager.IsGamePaused)
            {
                GameManager.ResumeGame();
            }
            else
            {
                GameManager.PauseGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("TAB Pressed");
            if (!GameManager.IsOnNormalPauseScreen)
            {
                if (GameManager.IsGamePaused)
                {
                    GameManager.HideArtifactScreen();
                }
                else
                {
                    GameManager.ShowArtifactScreen();
                    GetComponentInChildren<ArtifactScreen>().PopulateSlots();
                    UpdateSelectedItems();
                }
            }
        }

        HealthBar.value = Player._playerComponent.Health;
        if (Player.GetShieldManagerReference().ShieldEnabled)
        {
            ShieldHealthContainer.SetActive(true);
            ShieldHealth.value = Player.GetShieldManagerReference().ShieldHealth;
            ShieldHealth.maxValue = Player.GetShieldManagerReference().ShieldMaxHealth;
        }
        else
        {
            ShieldHealthContainer.SetActive(false);
        }

    }

    public void UpdateSelectedItems()
    {
        if (Player._inventoryController.EquippedItems[0])
        {
            ItemSelectionSlotA.GetComponent<ItemSlot>().SetDisplayedImage(Player._inventoryController.EquippedItems[0].ItemTextures[Player._inventoryController.EquippedItems[0]._itemTier]);
        }
        else
        {
            ItemSelectionSlotA.GetComponent<ItemSlot>().ClearDisplayedImage();
        }
        if (Player._inventoryController.EquippedItems[1])
        {
            ItemSelectionSlotB.GetComponent<ItemSlot>().SetDisplayedImage(Player._inventoryController.EquippedItems[1].ItemTextures[Player._inventoryController.EquippedItems[1]._itemTier]);
        }
        else
        {
            ItemSelectionSlotB.GetComponent<ItemSlot>().ClearDisplayedImage();
        }
    }

    public void OnArtifactSelected(int index)
    {
        Debug.Log("Slot " + index + " was clicked");
        if (Player._inventoryController.Unlocks.UnlockedItems[index])
        {
            if (!Player._inventoryController.EquippedItems.Contains(Player._inventoryController.Unlocks.UnlockedItems[index]))
            {
                Player._inventoryController.EquipItem(Player._inventoryController.Unlocks.UnlockedItems[index].ItemID);
            }
            else
            {
                
            }
        }
        UpdateSelectedItems();
        GetComponent<AudioSource>().PlayOneShot(ItemSelectSound);
    }

    public void OnArtifactHoverEnter(int index)
    {
        if (ArtifactScreen.GetComponent<ArtifactScreen>().InventorySlots[index].GetComponent<ItemSlot>().IsOccupied)
        {
            GetComponent<AudioSource>().PlayOneShot(ItemHoverSound);
        }
        //ArtifactScreen.GetComponent<ArtifactScreen>().InventorySlots[index].GetComponent<ItemSlot>().ShowSelectionGraphic();
    }

    public void OnArtifactHoverExit(int index)
    {
        //ArtifactScreen.GetComponent<ArtifactScreen>().InventorySlots[index].GetComponent<ItemSlot>().HideSelecionGraphic();
    }
}
