using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AttackController : MonoBehaviour
{
    public bool GunStickEnabled = true;
    public Animator PlayerAnimator;
    [SerializeField] float GunStickDefaultFireRate = 0.5f;
    [SerializeField] private float GattlingGunFireRateModifier = 0.2f;
    [SerializeField] float PlungerAttackRate = 1f;
    private float WeaponCooldown = 0.0f;
    public float PlungerAttackRange = 1f;
    public float PlungerDamage = 35f;
    [SerializeField] private float DeathRayBeamMaxRange = 200f;
    [SerializeField] public GameObject GunStick;
    [SerializeField] public GameObject Plunger;
    [SerializeField] public GameObject LaserPrefab;
    [SerializeField] public AudioClip GunstickFireSound;
    [SerializeField] public AudioClip PlungerQuickAttackClip;
    [SerializeField] public GameObject GattlingGunModel;
    [SerializeField] public AudioClip GattlingGunWindupSound;
    [SerializeField] public float GattlingGunWindupTime = 0.33f;
    [SerializeField] public bool EnableGattlingGun;
    [SerializeField] public float GattlingGunSpinSpeed = 11.25f;
    [SerializeField] public GameObject GattlingGunEmitter;
    private float GattlingGunCurrentTime = 0.0f;
    private AudioSource weaponSoundSource;
    public uint LaserType = 0;
    private LineRenderer RaygunLine;
    [SerializeField] private AudioClip RaygunBeamFireOpen;
    [SerializeField] private AudioClip RaygunBeamFireLoop;
    [SerializeField] private AudioClip RaygunBeamHit;

    [SerializeField] private float GunTurnTime = 0.2f;
    [SerializeField] private float LockOnBeamDuration = 0.66f;
    [SerializeField] private LockOnReticle reticleObject;
    [SerializeField] private float LockOnThreshold = 0.3f;
    [SerializeField] private float LockOnBaseTime = 1.5f;
    [SerializeField] private float LockOnProgress = 0f;
    [SerializeField] private bool LockOnReady = false;
    [SerializeField] private GameObject LockOnTarget = null;
    public float lockOnTime = 0.0f;
    public bool lockOnStarted = false;
    public bool LockOnEnabled = false;
    [SerializeField] private AudioClip _lockOnEnableSound;
    [SerializeField] private AudioClip _lockOnReadySound;
    [SerializeField] private AudioClip _lockOnStartSound;
    [SerializeField] private AudioClip _lockOnCancelSound;
    [SerializeField] private GameObject _lockOnGlyphPrefab;
    private bool hasPlayedLockOnReadySound = false;


    private float inputTimer = 0.0f;

    void Awake()
    {
        RaygunLine = GetComponent<LineRenderer>();
    }

    void Start()
    {
        RaygunLine.enabled = false;
        weaponSoundSource = gameObject.AddComponent<AudioSource>();
        GattlingGunEmitter.SetActive(false);
    }

    void Update()
    {
        if (GameManager.IsGamePaused)
            return;

        if (WeaponCooldown > 0)
        {
            WeaponCooldown -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1"))
        {
            if (GunStickEnabled)
            {
                if (EnableGattlingGun)
                {
                    HandleGattlingGunActions();
                }
                else
                {
                    if (LockOnEnabled)
                    {
                        lockOnTime += Time.deltaTime;
                        if (lockOnTime >= LockOnThreshold && !lockOnStarted)
                        {
                            lockOnStarted = true;
                            Debug.Log("Start Lock on");
                        }

                        if (lockOnTime >= LockOnThreshold && !LockOnTarget)
                        {
                            Debug.Log("Lock on cancelled, no target");
                            weaponSoundSource.PlayOneShot(_lockOnCancelSound);
                            LockOnEnabled = false;
                        }
                    }
                    else
                    {
                        HandleGunStick();
                    }
                }
            }
            else
            {
                PlungerQuickAttack();
            }
        }
        else
        {
            GattlingGunCurrentTime = 0.0f;
        }

        if (LockOnEnabled)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                lockOnStarted = false;
                // Check if the mouse is over an NPC's collider
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("NPC")) // Replace "NPC" with your NPC tag
                    {
                        lockOnStarted = true;
                        LockOnTarget = hit.collider.gameObject;
                        LockOnProgress = 0f;
                        reticleObject.SetLockOnTarget(LockOnTarget);
                    }
                    else
                    {
                        // Handle the case when the mouse is not over an NPC
                        LockOnProgress = 0f;
                        lockOnTime = 0.0f;
                        lockOnStarted = false;
                        LockOnTarget = null;
                    }
                }
            }

            if (lockOnStarted && LockOnTarget)
            {
                LockOnProgress += Time.deltaTime;
            }
            else
            {
                LockOnProgress = 0.0f;
                lockOnStarted = false;
                reticleObject.ClearLockOnTarget();
            }
        }
        else
        {
            LockOnProgress = 0.0f;
            lockOnStarted = false;
            reticleObject.ClearLockOnTarget();
        }

        if (lockOnStarted)
        {
            LockOnProgress += Time.deltaTime;
            if (LockOnProgress >= LockOnBaseTime)
            {
                LockOnReady = true;
                reticleObject.LockOnValue = 1;
                if (!hasPlayedLockOnReadySound)
                {
                    weaponSoundSource.PlayOneShot(_lockOnReadySound);
                    hasPlayedLockOnReadySound = true; // Set the flag to true
                }
            }
            else
            {
                LockOnReady = false;
                reticleObject.LockOnValue = LockOnProgress / LockOnBaseTime;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (LockOnEnabled)
            {
                if (LockOnReady && LockOnTarget != null)
                {
                    Debug.Log("Fire at locked on target, which is a " + LockOnTarget.name);
                    var targetPosition = LockOnTarget.transform.position;
                    targetPosition.y++;
                    GetComponent<GunStickAimController>().AimGunstickTowards(targetPosition, GunTurnTime, LockOnBeamDuration);
                }
                if (lockOnTime < LockOnThreshold)
                {
                    weaponSoundSource.PlayOneShot(_lockOnCancelSound);
                    HandleGunStick();
                }
            }

            LockOnReady = false;
            lockOnStarted = false;
            lockOnTime = 0.0f;
            LockOnProgress = 0f;
            LockOnTarget = null;
            hasPlayedLockOnReadySound = false;
            reticleObject.ClearLockOnTarget();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            LockOnEnabled = true;
            weaponSoundSource.PlayOneShot(_lockOnEnableSound);
            LockOnProgress = 0f;
            LockOnReady = false;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            LockOnEnabled = false;
            weaponSoundSource.PlayOneShot(_lockOnCancelSound);
            LockOnProgress = 0f;
            LockOnReady = false;
            LockOnTarget = null;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.IsGamePaused)
        {
            GattlingGunModel.SetActive(EnableGattlingGun);
        }
    }

    void HandleGattlingGunActions()
    {
        weaponSoundSource.volume = 0.66f;

        if (GattlingGunCurrentTime == 0.0f)
            weaponSoundSource.PlayOneShot(GattlingGunWindupSound);

        if (GattlingGunCurrentTime < GattlingGunWindupTime)
        {
            GattlingGunCurrentTime += Time.deltaTime;
            return;
        }

        if (WeaponCooldown > 0)
        {
            return;
        }

        FireGunStick(LaserType);
    }

    public void HandleGunStick()
    {
        weaponSoundSource.volume = 0.8f;
        FireGunStick(LaserType);
        GattlingGunEmitter.SetActive(false);
    }

    public void HandleGunStickBeam()
    {
        if (WeaponCooldown > 0)
        {
            return;
        }
        
        weaponSoundSource.volume = 0.5f;
        RaycastHit hit;
        Vector3 fwd = GunStick.transform.TransformDirection(Vector3.right);
        RaygunLine.SetPosition(0, GunStick.transform.position);
        Debug.DrawRay(GunStick.transform.position, fwd, Color.green, 10f);
        if (Physics.Raycast(GunStick.transform.position, fwd, out hit, DeathRayBeamMaxRange))
        {
            RaygunLine.SetPosition(1, hit.point);
            RaygunLine.enabled = true;
            weaponSoundSource.volume = 1f;
            weaponSoundSource.PlayOneShot(RaygunBeamFireOpen);
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit: " + hit.collider.gameObject.name);
            var npcAI = hit.collider.gameObject.GetComponent<BaseAI>();
            if (npcAI != null)
            {
                DamageInfo info = new DamageInfo(npcAI.MaxHealth, gameObject, DamageType.DeathRay);
                npcAI.Damage(info);
                AudioSource.PlayClipAtPoint(RaygunBeamHit, hit.point, 1f);
            }

        }
        else
        {
            RaygunLine.SetPosition(1, GunStick.transform.position + fwd*DeathRayBeamMaxRange);
            RaygunLine.enabled = true;
            weaponSoundSource.volume = 1f;
            weaponSoundSource.PlayOneShot(RaygunBeamFireOpen);
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit nothing");
        }

        WeaponCooldown = 1f;
    }

    IEnumerator HideDeathRayBeam(float duration)
    {
        GetComponent<Movement>().MovementEnabled = false;
        weaponSoundSource.clip = RaygunBeamFireLoop;
        weaponSoundSource.loop = true;
        weaponSoundSource.Play();
        yield return new WaitForSeconds(duration);
        RaygunLine.enabled = false;
        RaygunLine.SetPosition(0, transform.position);
        RaygunLine.SetPosition(1, transform.position);
        weaponSoundSource.Stop();
        weaponSoundSource.loop = false;
        GetComponent<Movement>().MovementEnabled = true;
    }

    private void PlungerQuickAttack()
    {
        if (WeaponCooldown > 0)
        {
            return;
        }
        RaycastHit hit;
        Vector3 fwd = Plunger.transform.TransformDirection(Vector3.forward);
        weaponSoundSource.PlayOneShot(PlungerQuickAttackClip);
        PlayerAnimator.Play(Animator.StringToHash("PlungerAttack"), -1, 0);
        WeaponCooldown = PlungerAttackRate;
        if (Physics.Raycast(Plunger.transform.position, Plunger.transform.forward * -1, out hit, PlungerAttackRange))
        {
            Debug.Log("Plunger hit: " + hit.collider.gameObject.name);
            var npcAI = hit.collider.gameObject.GetComponent<BaseAI>();
            if (npcAI != null)
            {
                DamageInfo info = new DamageInfo(PlungerDamage, gameObject, DamageType.Plunger);
                npcAI.Damage(info);
                var playerComponent = GetComponent<PlayerComponent>();
                if (playerComponent.Health < playerComponent.MaxHealth)
                {
                    playerComponent.Health += ((playerComponent.MaxHealth - playerComponent.Health) * 0.05f);
                    if (playerComponent.Health > playerComponent.MaxHealth)
                    {
                        playerComponent.Health = playerComponent.MaxHealth;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Plunger hit nothing");
        }
    }

    private void FireGunStick(uint AttackStrength)
    {
        if (WeaponCooldown > 0)
        {
            return;
        }
        weaponSoundSource.volume = 0.5f;
        weaponSoundSource.PlayOneShot(GunstickFireSound);
        WeaponCooldown = EnableGattlingGun ? GunStickDefaultFireRate * GattlingGunFireRateModifier : GunStickDefaultFireRate;
        GameObject go = Instantiate(LaserPrefab, GunStick.transform.position, GunStick.transform.rotation);
        go.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
        go.GetComponent<EnergyDischargeController>().SetData(AttackStrength);
        GameObject.Destroy(go, 3f);
    }
}