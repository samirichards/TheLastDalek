using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _baseSpeed = 3f;
    [SerializeField] public float EnhancedSpeedModifier = 1.5f;
    [SerializeField] private float _turnSpeed = 360;
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


    //TODO Come back here and finish elevate behavior

    private void Update()
    {
        if (!GameManager.IsGamePaused)
        {
            GatherInput();
            Look();
        }
    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource2 = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        ElevationTargets = new List<GameObject>();
        propController = GetComponentInChildren<PropController>();
    }

    void Start()
    {
        _baseSpeed = _baseSpeed * Player._PropController.getMovementSpeedMultiplier;
    }

    private void FixedUpdate()
    {
        if (!GameManager.IsGamePaused)
        {
            Move();
            propController.MovementAudio(IsMoving, IsElevating, IsMovementEnhanced, wasElevating);
            HandleElevate();
        }
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

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
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
            _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * finalSpeed * Time.deltaTime);
            IsMoving = _input.normalized.magnitude > 0.05f;
        }
        else
        {
            IsMoving = false;
        }
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}