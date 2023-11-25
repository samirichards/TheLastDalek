using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropController : MonoBehaviour
{
    public enum SoundClips
    {
        RaygunFire,
        PlungerAttack,
        ExterminateVO,
        DamageSFX,
        DeathVO,
        ChantVO,
        StartUp
    }

    public enum AnimationClips
    {
        PlungerAttack,
        Death,
        StartUp,
        Stun
    }

    [Header("Physical Components ---")]
    [SerializeField] private GameObject _plungerObject;
    [SerializeField] private GameObject _gunStickObject;
    [SerializeField] private GameObject _bodyBase;
    [SerializeField] private GameObject _centerObject;
    [SerializeField] private GameObject _headObject;
    [SerializeField] private GameObject _eyeStalkObject;
    [SerializeField] private GameObject _gattlingGunHolderObject;
    [SerializeField] private GameObject[] _lightClusterGroup;

    [Header("Audio ---")]
    [SerializeField] private AudioSource SpeechAudioSource;
    [SerializeField] private AudioSource GeneralAudioSource;
    [SerializeField] private AudioSource MovementAudioSource;
    [SerializeField] private AudioClip[] _rayGunQuickFire;
    [SerializeField] private AudioClip[] _plungerQuickAttack;
    [SerializeField] private AudioClip RaygunBeamFireOpen;
    [SerializeField] private AudioClip RaygunBeamFireLoop;
    [SerializeField] public AudioClip RaygunBeamHit;
    [SerializeField] private AudioClip[] _exterminateVO;
    [SerializeField] private AudioClip[] _damageSFX;
    [SerializeField] private AudioClip[] _deathVO;
    [SerializeField] private AudioClip[] _chantVO;
    [SerializeField] private AudioClip StartUpClip;

    [Header("Audio (Movement) ---")]
    [SerializeField] private AudioClip MovementStart;
    [SerializeField] private AudioClip MovementLoop;
    [SerializeField] private AudioClip MovementEnd;
    [SerializeField] private AudioClip ElevateStart;
    [SerializeField] private AudioClip ElevateLoop;
    [SerializeField] private AudioClip ElevateEnd;
    [SerializeField] private float MovementVolume = 1.0f;
    [SerializeField] private float ElevateVolume = 1.0f;

    [Header("Animations ---")] 
    [SerializeField] private Animator _animator;

    [Header("Other Stats ---")] 
    [SerializeField] private GameObject InteractionPoint;
    [SerializeField] private GameObject _rayGunDischargePrefab;
    [SerializeField] private Color _rayGunBeamColour;
    [SerializeField] private Material _rayGunBeamMaterial;
    [SerializeField] private float _pointsMultiplier = 1.0f;
    [SerializeField] private float _movementSpeedMultiplier = 1.0f;
    [SerializeField] private float _damageMultiplier = 1.0f;
    [SerializeField] private float _stealthMultiplier = 1.0f;
    [SerializeField] private float MaxHealthMultiplier = 1.0f;
    [SerializeField] private AttackController.AttackTypes _attackType = AttackController.AttackTypes.Standard;

    [Header("Static Display Settings")] 
    [SerializeField] private bool InDisplayMode = false;

    void Start()
    {
        if (InDisplayMode)
        {
            _animator.enabled = true;
            _animator.SetBool("DisplayMode", true);
        }
    }


    public void PlaySoundClip(SoundClips clipType)
    {
        switch (clipType)
        {
            case SoundClips.ExterminateVO:
                int SelectedExterminateVOClip = Random.Range(0, _exterminateVO.Length);
                SpeechAudioSource.PlayOneShot(_exterminateVO[SelectedExterminateVOClip]);
                StartCoroutine(VaryLightIntensity(_exterminateVO[SelectedExterminateVOClip].length, _lightClusterGroup));
                break;
            case SoundClips.ChantVO:
                int SelectedChantVoClip = Random.Range(0, _chantVO.Length);
                SpeechAudioSource.PlayOneShot(_chantVO[SelectedChantVoClip]);
                StartCoroutine(VaryLightIntensity(_chantVO[SelectedChantVoClip].length, _lightClusterGroup));
                break;
            case SoundClips.DeathVO:
                int SelectedDeathVO = Random.Range(0, _deathVO.Length);
                SpeechAudioSource.PlayOneShot(_deathVO[SelectedDeathVO]);
                StartCoroutine(VaryLightIntensity(_deathVO[SelectedDeathVO].length, _lightClusterGroup));
                break;
            case SoundClips.DamageSFX:
                int SelectedDamageSFXClip = Random.Range(0, _damageSFX.Length);
                GeneralAudioSource.PlayOneShot(_damageSFX[SelectedDamageSFXClip]);
                break;
            case SoundClips.RaygunFire:
                int SelectedRayGunFireClip = Random.Range(0, _rayGunQuickFire.Length);
                GeneralAudioSource.PlayOneShot(_rayGunQuickFire[SelectedRayGunFireClip]);
                break;
            case SoundClips.PlungerAttack:
                int SelectedPlungerAttackClip = Random.Range(0, _plungerQuickAttack.Length);
                GeneralAudioSource.PlayOneShot(_plungerQuickAttack[SelectedPlungerAttackClip]);
                break;
            case SoundClips.StartUp:
                GeneralAudioSource.PlayOneShot(StartUpClip);
                break;
            default:
                break;
        }
    }

    public void PlayAnimationClip(AnimationClips clip)
    {
        switch (clip)
        {
            case AnimationClips.PlungerAttack:
                _animator.enabled = true;
                PlaySoundClip(SoundClips.PlungerAttack);
                _animator.SetTrigger("PlungerAttack");
                break;
            case AnimationClips.StartUp:
                _animator.enabled = true;
                _animator.SetTrigger("StartUp");
                break;
            case AnimationClips.Stun:
                break;
            case AnimationClips.Death:
                PlaySoundClip(SoundClips.DeathVO);
                break;
        }
    }

    public void StopAnimator()
    {
        _animator.enabled = false;
    }

    public void StartAnimator()
    {
        _animator.enabled = true;
    }

    public void PlayHitSoundatPoint(Vector3 location)
    {
        AudioSource.PlayClipAtPoint(RaygunBeamHit, location);
    }

    public IEnumerator PlayExterminationLoop(float duration)
    {
        GeneralAudioSource.PlayOneShot(RaygunBeamFireOpen);
        GeneralAudioSource.clip = RaygunBeamFireLoop;
        GeneralAudioSource.loop = true;
        GeneralAudioSource.Play();
        yield return new WaitForSeconds(duration);
        GeneralAudioSource.Stop();
        GeneralAudioSource.loop = false;
    }

    public void MovementAudio(bool IsMoving, bool IsElevating, bool IsMovementEnhanced, bool wasElevating)
    {
        if (IsElevating && GameManager.IsGamePaused == false)
        {
            // Play Elevate sounds continuously
            MovementAudioSource.clip = ElevateLoop;
            MovementAudioSource.loop = true;
            MovementAudioSource.volume = MovementVolume;

            if (!MovementAudioSource.isPlaying)
            {
                MovementAudioSource.volume = ElevateVolume;
                MovementAudioSource.PlayOneShot(ElevateStart);
                MovementAudioSource.PlayDelayed(ElevateStart.length - 0.1f);
                if (IsMovementEnhanced)
                {
                    MovementAudioSource.pitch = 1.2f;
                }
                else
                {
                    MovementAudioSource.pitch = 1f;
                }
            }
        }
        else if (IsMoving && GameManager.IsGamePaused == false)
        {
            // Play normal Movement sounds
            MovementAudioSource.clip = MovementLoop;
            MovementAudioSource.loop = true;
            MovementAudioSource.volume = MovementVolume;

            if (!MovementAudioSource.isPlaying)
            {
                MovementAudioSource.volume = MovementVolume;
                MovementAudioSource.PlayOneShot(MovementStart);
                MovementAudioSource.PlayDelayed(MovementStart.length - 0.1f);
                if (IsMovementEnhanced)
                {
                    MovementAudioSource.pitch = 1.2f;
                }
                else
                {
                    MovementAudioSource.pitch = 1f;
                }
            }
        }
        else
        {
            // Stop all sounds when not moving or not elevating
            //TODO come back and fix this, refactoring broke it
            if (MovementAudioSource.isPlaying)
            {
                MovementAudioSource.Stop();
                /*
                if (wasElevating)
                {
                    // ElevateEnd sound should play here
                    MovementAudioSource.volume = MovementVolume;
                    MovementAudioSource.PlayOneShot(ElevateEnd);
                }
                else
                {
                    // MovementEnd sound should play here if MovementLoop was playing
                    MovementAudioSource.volume = MovementVolume;
                    MovementAudioSource.PlayOneShot(MovementEnd);
                }
                */
            }
        }
    }

    public void StopSound()
    {
        GeneralAudioSource.loop = false;
        GeneralAudioSource.Stop();
        MovementAudioSource.loop = false;
        MovementAudioSource.Stop();
        SpeechAudioSource.loop = false;
        SpeechAudioSource.Stop();
    }

    private IEnumerator VaryLightIntensity(float duration, GameObject[] lightSources)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        foreach (Light bulb in lightSources.Select(a=>a.GetComponent<Light>()))
        {
            bulb.enabled = true;
        }

        while (Time.time < endTime)
        {
            // Use the normalized time to vary the light intensity
            float amplitude = GetAudioAmplitude(); // Implement a method to get audio amplitude
            float intensity = Mathf.Lerp(0f, 1f, amplitude);

            // Apply the intensity to the light source
            foreach (Light bulb in lightSources.Select(a => a.GetComponent<Light>()))
            {
                bulb.intensity = intensity * 10;
            }

            yield return null; // Wait for the next frame
        }

        foreach (Light bulb in lightSources.Select(a => a.GetComponent<Light>()))
        {
            bulb.enabled = false;
        }
    }

    private float GetAudioAmplitude()
    {
        float[] spectrum = new float[256];
        SpeechAudioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        float amplitude = 0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            amplitude += spectrum[i];
        }

        amplitude /= spectrum.Length;
        amplitude = amplitude * 5000;
        if (amplitude < 1)
        {
            return 0;
        }
        return amplitude;
    }

    public GameObject getPlungerObject => _plungerObject;
    public GameObject getGunStickObject => _gunStickObject;
    public GameObject getBodyBase => _bodyBase;
    public GameObject getCenterObject => _centerObject;
    public GameObject getHeadObject => _headObject;
    public GameObject getEyeStalkObject => _eyeStalkObject;
    public GameObject getGattlingGunHolderObject => _gattlingGunHolderObject;
    public GameObject getInteractionPoint => InteractionPoint;
    public AudioSource getGeneralAudioSource => GeneralAudioSource;

    public GameObject getRayGunDischargePrefab => _rayGunDischargePrefab;
    public Color getRayGunBeamColour => _rayGunBeamColour;
    public Material getRayGunBeamMaterial => _rayGunBeamMaterial;
    public float getPointsMultiplier => _pointsMultiplier;
    public float getMovementSpeedMultiplier => _movementSpeedMultiplier;
    public float getDamageMultiplier => _damageMultiplier;
    public float getStealthMultiplier => _stealthMultiplier;
    public float getHealthMultiplier => MaxHealthMultiplier;
    public AttackController.AttackTypes getAttackType => _attackType;
}
