using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

    [SerializeField] public GameObject _PlayerPrefab; 
    private static GameObject PlayerPrefab;

    private static ShieldManager _shieldManager;
    public static Player Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new Player();
                playerObjectReference = GameObject.FindGameObjectWithTag("Player");

                //I am very tired
                //TODO make this do what you think you want it to do (idk anymore)
                if (playerObjectReference == null)
                {
                    playerObjectReference = Instantiate(PlayerPrefab);
                    playerObjectReference.name = PlayerPrefab.name;
                }


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
        _shieldManager = playerObjectReference.GetComponentInChildren<ShieldManager>();
        _inventoryController = playerObjectReference.GetComponentInChildren<InventoryController>();
    }

    public static ShieldManager GetShieldManagerReference()
    {
        return _shieldManager;
    }

    public static Movement GetMovementController()
    {
        return _movement;
    }

    void Start()
    {

    }
}
