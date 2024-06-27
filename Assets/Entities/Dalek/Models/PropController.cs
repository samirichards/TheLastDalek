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
        Stun,
        Elevate
    }

    [Header("Physical Components ---")]
    [SerializeField] private GameObject _plungerObject;
    [SerializeField] private GameObject _gunStickObject;
    [SerializeField] private GameObject _bodyBase;
    [SerializeField] private GameObject _centerObject;
    [SerializeField] private GameObject _headObject;
    [SerializeField] private GameObject _eyeStalkObject;
    [SerializeField] private GameObject _gattlingGunHolderObject;
    [SerializeField] private Light[] _lightClusterGroup;

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
    [SerializeField] private bool isElevating = false;

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


    private bool wasMoving = false;

    void Start()
    {
        if (InDisplayMode)
        {
            _animator.enabled = true;
            _animator.SetBool("DisplayMode", true);
        }
        StartBehavior();
    }

    public virtual void OnFire()
    {
        Debug.Log(gameObject.name + " OnFire() default behavior");
    }

    public virtual void OnPreFire()
    {
        Debug.Log(gameObject.name + " OnPreFire() default behavior");
    }

    public virtual void OnRapidFire()
    {
        Debug.Log(gameObject.name + " OnRapidFire() default behavior");
    }

    public virtual void OnRapidFireEnd()
    {
        Debug.Log(gameObject.name + " OnRapidFireEnd() default behavior");
    }

    public virtual void OnMelee()
    {
        Debug.Log(gameObject.name + " OnMelee() default behavior");
    }

    public virtual void OnHit(Vector3 hitOriginDirection)
    {
        Debug.Log(gameObject.name + " OnHit() default behavior, hit from " + hitOriginDirection.normalized);
    }

    public virtual void StartBehavior()
    {

    }

    public void SetElevation(bool state)
    {
        isElevating = state;
        _animator.enabled = isElevating;
        _animator.SetBool("IsElevating", isElevating);
        //TODO finish
    }


    public void PlaySoundClip(SoundClips clipType)
    {
        switch (clipType)
        {
            case SoundClips.ExterminateVO:
                int SelectedExterminateVOClip = Random.Range(0, _exterminateVO.Length);
                SpeechAudioSource.PlayOneShot(_exterminateVO[SelectedExterminateVOClip]);
                StartCoroutine(EmitterVOFlash(_exterminateVO[SelectedExterminateVOClip].length));
                break;
            case SoundClips.ChantVO:
                int SelectedChantVoClip = Random.Range(0, _chantVO.Length);
                SpeechAudioSource.PlayOneShot(_chantVO[SelectedChantVoClip]);
                StartCoroutine(EmitterVOFlash(_chantVO[SelectedChantVoClip].length));
                break;
            case SoundClips.DeathVO:
                int SelectedDeathVO = Random.Range(0, _deathVO.Length);
                SpeechAudioSource.PlayOneShot(_deathVO[SelectedDeathVO]);
                StartCoroutine(EmitterVOFlash(_deathVO[SelectedDeathVO].length));
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
                _animator.enabled = true;
                _animator.Play("Death");
                break;
            case AnimationClips.Elevate:
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
            wasMoving = true;

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
            if (MovementAudioSource.isPlaying && (!IsMoving || !isElevating))
            {
                MovementAudioSource.Stop();

                if (wasElevating)
                {
                    // ElevateEnd sound should play here
                    MovementAudioSource.volume = MovementVolume;
                    MovementAudioSource.PlayOneShot(ElevateEnd);
                }
                if(wasMoving)
                {
                    // MovementEnd sound should play here if MovementLoop was playing
                    //MovementAudioSource.volume = MovementVolume;
                    //MovementAudioSource.PlayOneShot(MovementEnd);
                    //Little hack to make it always play over whatever is causing it to stop when it shouldn't
                    AudioSource.PlayClipAtPoint(MovementEnd, transform.position, MovementVolume);
                    wasMoving = false;
                }
                
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
        if (wasMoving)
        {
            MovementAudioSource.volume = MovementVolume;
            MovementAudioSource.PlayOneShot(MovementEnd);
            wasMoving = false;
        }
    }

    private IEnumerator VaryLightIntensity(float duration, Light[] lightSources)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        foreach (Light bulb in lightSources)
        {
            bulb.enabled = true;
        }

        while (Time.time < endTime)
        {
            // Use the normalized time to vary the light intensity
            float amplitude = GetAudioAmplitude(); // Implement a method to get audio amplitude
            float intensity = Mathf.Lerp(0f, 1f, amplitude);

            // Apply the intensity to the light source
            foreach (Light bulb in lightSources)
            {
                bulb.intensity = intensity * 10;
            }

            yield return null; // Wait for the next frame
        }

        foreach (Light bulb in lightSources)
        {
            bulb.enabled = false;
        }
    }

    protected float GetAudioAmplitude()
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

    protected virtual IEnumerator EmitterVOFlash(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        SetEmittersActive(false);

        while (Time.time < endTime)
        {
            // Use the normalized time to vary the light intensity
            float amplitude = GetAudioAmplitude(); // Implement a method to get audio amplitude
            float intensity = Mathf.Lerp(0f, 1f, amplitude);

            if (intensity > 0.25f)
            {
                SetEmittersActive(true);
            }
            else
            {
                SetEmittersActive(false);
            }

            yield return null; // Wait for the next frame
        }

        SetEmittersActive(false);
    } 

    //The only dalek type which doesn't override this behavior is the Standard dalek, every other type works by changing the material on the emitter bulbs, maybe the standard dalek should follow, it would look more realistic
    public virtual void SetEmittersActive(bool state)
    {
        foreach (Light bulb in _lightClusterGroup)
        {
            bulb.enabled = state;
        }
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
    public bool getElevating => isElevating;
}
