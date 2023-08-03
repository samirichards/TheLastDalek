using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private static Player _Instance;
    private static GameObject playerObjectReference;
    [SerializeField] private static InventoryManager _inventoryManager;
    [SerializeField] private static Movement _movement;
    [SerializeField] private static BoxCollider _collider;
    [SerializeField] private static Rigidbody _rb;
    [SerializeField] private static SpeechController _speechController;
    [SerializeField] private static LookAtAnimator _lookAtAnimator;
    [SerializeField] private static ChestRotateController _chestRotateController;
    [SerializeField] private static AttackController _attackController;
    [SerializeField] private static PlayerComponent _playerComponent;
    [SerializeField] private static Animator _animator;
    [SerializeField] private static GameManager _gameManager;
    [SerializeField] private static CursorControl _cursorControl;
    [SerializeField] private static InteractionController _interactionController;
    private static ShieldManager _shieldManager;
    public static Player Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new Player();
                playerObjectReference = GameObject.Find("Player");
                _Instance = playerObjectReference.GetComponent<Player>();
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(playerObjectReference);
            }
            return _Instance;
        }
    }

    public static GameObject GetPlayerReference()
    {
        return playerObjectReference;
    }

    void Awake()
    {
        playerObjectReference = GameObject.Find("Player");
        _inventoryManager = playerObjectReference.GetComponent<InventoryManager>();
        _movement = playerObjectReference.GetComponent<Movement>();
        _collider = playerObjectReference.GetComponent<BoxCollider>();
        _rb = playerObjectReference.GetComponent<Rigidbody>();
        _speechController = playerObjectReference.GetComponent<SpeechController>();
        _lookAtAnimator = playerObjectReference.GetComponent<LookAtAnimator>();
        _chestRotateController = playerObjectReference.GetComponent<ChestRotateController>();
        _attackController = playerObjectReference.GetComponent<AttackController>();
        _playerComponent = playerObjectReference.GetComponent<PlayerComponent>();
        _animator = playerObjectReference.GetComponent<Animator>();
        _gameManager = playerObjectReference.GetComponent<GameManager>();
        _cursorControl = playerObjectReference.GetComponent<CursorControl>();
        _interactionController = playerObjectReference.GetComponent<InteractionController>();
        _shieldManager = playerObjectReference.GetComponentInChildren<ShieldManager>();
    }

    public static ShieldManager GetShieldManagerReference()
    {
        return _shieldManager;
    }

    public static InventoryManager GetInventoryManagerReference()
    {
        return _inventoryManager;
    }

    public static Movement GetMovementController()
    {
        return _movement;
    }

    void Start()
    {

    }
}
