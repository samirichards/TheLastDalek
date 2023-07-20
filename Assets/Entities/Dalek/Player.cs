using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private static Player _Instance;
    private static GameObject playerObjectReference;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private Movement _movement;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SpeechController _speechController;
    [SerializeField] private LookAtAnimator _lookAtAnimator;
    [SerializeField] private ChestRotateController _chestRotateController;
    [SerializeField] private AttackController _attackController;
    [SerializeField] private PlayerComponent _playerComponent;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private CursorControl _cursorControl;
    [SerializeField] private InteractionController _interactionController;
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
    }

    void Start()
    {

    }
}
