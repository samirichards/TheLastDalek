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
    private bool IsMoving = false;
    [SerializeField] public bool IsMovementEnhanced = false;
    [SerializeField] private float MovementVolume = 0.33f;
    private AudioSource audioSource;
    private AudioSource audioSource2;
    private Vector3 _input;
    private GameManager _gameManagerComponent;

    private void Update()
    {
        if (!_gameManagerComponent.IsGamePaused)
        {
            GatherInput();
            Look();
        }
    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource2 = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        _gameManagerComponent = GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        if (!_gameManagerComponent.IsGamePaused)
        {
            Move();
            MovementAudio();
        }
    }

    private void MovementAudio()
    {
        audioSource.clip = MovementLoop;
        audioSource.loop = true;
        if (IsMoving && _gameManagerComponent.IsGamePaused == false)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = MovementVolume;
                audioSource.PlayOneShot(MovementStart);
                audioSource.PlayDelayed(MovementStart.length - 0.1f);
                if (IsMovementEnhanced)
                {
                    audioSource.pitch = 1.2f;
                }
                else
                {
                    audioSource.pitch = 1f;
                }
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource2.volume = MovementVolume;
                audioSource2.PlayOneShot(MovementEnd);
            }
        }
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    public void StopSound()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        float finalSpeed = _baseSpeed;
        if (IsMovementEnhanced)
        {
            finalSpeed = _baseSpeed * EnhancedSpeedModifier;
        }
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * finalSpeed * Time.deltaTime);
        IsMoving = _input.normalized.magnitude > 0.05f;
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}