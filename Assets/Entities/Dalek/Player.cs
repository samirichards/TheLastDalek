using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

[ExecuteInEditMode]
public class Player : MonoBehaviour
{
    private static Player _Instance;
    private static GameObject playerObjectReference;
    //[SerializeField] private static InventoryManager _inventoryManager;
    [SerializeField] public static Movement _movement;
    [SerializeField] public static BoxCollider _collider;
    [SerializeField] public static Rigidbody _rb;
    [SerializeField] public static SpeechController _speechController;
    [SerializeField] public static LookAtAnimator _lookAtAnimator;
    [SerializeField] public static ChestRotateController _chestRotateController;
    [SerializeField] public static AttackController _attackController;
    [SerializeField] public static PlayerComponent _playerComponent;
    [SerializeField] public static Animator _animator;
    [SerializeField] public static CursorControl _cursorControl;
    [SerializeField] public static InteractionController _interactionController;
    [SerializeField] public static InventoryController _inventoryController;
    [SerializeField] public static PropController _PropController;
    [Header("Selected Dalek Model ---")]
    [SerializeField] private SelectedDalek _selectedDalek = SelectedDalek.StandardDalek;
    [SerializeField] private GameObject[] DalekModels;
    public CameraObject cameraInstance;

    [SerializeField] public GameObject _PlayerPrefab; 
    private static GameObject PlayerPrefab;
    public static Player Instance
    {
        get
        {
            if (!_Instance)
            {
                playerObjectReference = GameObject.FindGameObjectWithTag("Player");
                _Instance = playerObjectReference.GetComponent<Player>();
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(playerObjectReference);
                
            }
            return _Instance;
        }
    }

    public enum SelectedDalek
    {
        StandardDalek,
        ImperialDalek,
        ParadigmDalek,
        GenesisDalek
    }

    public static GameObject GetPlayerReference()
    {
        return playerObjectReference;
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        UpdateModel();
    }
    public void UpdateModel()
    {
        foreach (GameObject model in DalekModels)
        {
            model.SetActive(false);
        }
        DalekModels[(int)_selectedDalek].SetActive(true);
    }

    

    void Awake()
    {
        if (_Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _Instance = this;

            PlayerPrefab = _PlayerPrefab;
            playerObjectReference = GameObject.Find("Player");
            //_inventoryManager = playerObjectReference.GetComponent<InventoryManager>();
            _movement = playerObjectReference.GetComponent<Movement>();
            _collider = playerObjectReference.GetComponent<BoxCollider>();
            _rb = playerObjectReference.GetComponent<Rigidbody>();
            _speechController = playerObjectReference.GetComponent<SpeechController>();
            _lookAtAnimator = playerObjectReference.GetComponent<LookAtAnimator>();
            _chestRotateController = playerObjectReference.GetComponent<ChestRotateController>();
            _attackController = playerObjectReference.GetComponent<AttackController>();
            _playerComponent = playerObjectReference.GetComponent<PlayerComponent>();
            _animator = playerObjectReference.GetComponent<Animator>();
            _cursorControl = playerObjectReference.GetComponent<CursorControl>();
            _interactionController = playerObjectReference.GetComponent<InteractionController>();
            _inventoryController = playerObjectReference.GetComponentInChildren<InventoryController>();
            _PropController = playerObjectReference.GetComponentInChildren<PropController>();
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static Movement GetMovementController()
    {
        return _movement;
    }

    void Start()
    {
        _inventoryController.RunUpdateAbilities();
        cameraInstance = GameObject.Find("CameraObject").GetComponent<CameraObject>();
    }
}
