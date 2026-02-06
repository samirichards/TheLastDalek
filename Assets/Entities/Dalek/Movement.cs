using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _baseSpeed = 3f;
    [SerializeField] public float EnhancedSpeedModifier = 1.5f;
    [SerializeField] private float _turnSpeed = 720;
    [SerializeField] private AudioClip MovementStart;
    [SerializeField] private AudioClip MovementLoop;
    [SerializeField] private AudioClip MovementEnd;
    [SerializeField] private AudioClip ElevateStart;
    [SerializeField] private AudioClip ElevateLoop;
    [SerializeField] private AudioClip ElevateEnd;
    private bool IsMoving = false;
    [SerializeField] public bool IsMovementEnhanced = false;
    [SerializeField] public float MovementVolume = 0.33f;
    [SerializeField] public float ElevateVolume = 0.45f;
    private AudioSource audioSource;
    private AudioSource audioSource2;
    private Vector3 _input;
    public bool CanElevate = false;
    public bool MovementEnabled = true;
    public bool IsElevating;
    private bool wasElevating = false;
    public List<GameObject> ElevationTargets;
    private PropController propController;
    private Vector3 _direction;


    //TODO Come back here and finish elevate behavior

    private void Update()
    {

    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource2 = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        ElevationTargets = new List<GameObject>();
        _controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        _baseSpeed = _baseSpeed * Player._PropController.getMovementSpeedMultiplier;
        propController = GetComponentInChildren<PropController>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.IsGamePaused)
        {
            if (MovementEnabled)
            {
                float finalSpeed = _baseSpeed;
                if (IsMovementEnhanced)
                {
                    finalSpeed = (_baseSpeed * EnhancedSpeedModifier);
                }
                Vector3 toConvert = new Vector3(_direction.x, 0.1f, _direction.y);
                _controller.SimpleMove(toConvert.ToIso() * finalSpeed * Time.deltaTime);
                //_rb.velocity = toConvert.ToIso() * finalSpeed * Time.deltaTime;
                //_rb.MovePosition(toConvert.ToIso() * finalSpeed * Time.deltaTime);
                IsMoving = _direction.normalized.magnitude > 0.05f;
                Look();
            }
            else
            {
                IsMoving = false;
            }
            propController.MovementAudio(IsMoving, IsElevating, IsMovementEnhanced, wasElevating);
        }
        HandleElevate();
    }

    private void HandleElevate()
    {
        if (ElevationTargets.Count > 0 && CanElevate)
        {
            IsElevating = true;
        }
        else
        {
            IsElevating = false;
        }

        // Check for transition from elevating to not elevating
        if (wasElevating && !IsElevating)
        {
            // ElevateEnd sound should play here
            audioSource2.volume = MovementVolume;
            audioSource2.PlayOneShot(ElevateEnd);
        }

        wasElevating = IsElevating; // Update the previous state
    }

    private void Look()
    {
        if (_direction == Vector3.zero) return;
        Vector3 toConvert = new Vector3(_direction.x, 0.1f, _direction.y);

        var rot = Quaternion.LookRotation(toConvert.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (MovementEnabled)
        {
            float finalSpeed = _baseSpeed;
            if (IsMovementEnhanced)
            {
                finalSpeed = _baseSpeed * EnhancedSpeedModifier;
            }
            _controller.Move(_input.ToIso() * finalSpeed * Time.deltaTime);
            //_rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * finalSpeed * Time.deltaTime);
            IsMoving = _input.normalized.magnitude > 0.05f;
        }
        else
        {
            IsMoving = false;
        }
        propController.MovementAudio(IsMoving, IsElevating, IsMovementEnhanced, wasElevating);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>() * 40;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void InputSystem_Actions.IPlayerActions.OnFocus(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}