using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class AttackController : MonoBehaviour
{
    // --- Serialized Fields ---
    [Header("GunStick Settings")]
    public bool GunStickEnabled = true;
    [SerializeField] public float GunStickDefaultFireRate = 0.5f;
    [SerializeField] private float GattlingGunFireRateModifier = 0.2f;
    [SerializeField] public GameObject GattlingGunModel;
    [SerializeField] public AudioClip GattlingGunWindupSound;
    [SerializeField] public float GattlingGunWindupTime = 0.33f;
    [SerializeField] public bool EnableGattlingGun;

    [Header("Plunger Settings")]
    [SerializeField] float PlungerAttackRate = 1f;
    public float PlungerAttackRange = 1.5f;
    public float PlungerDamage = 35f;
    public float PlungerHealthRechargePercentage = 0.05f;
    public float PlungerAttackConeSpread = 40;
    public float PlungerAttackConeHitCount = 5;
    [SerializeField] private float PlungerRange = 20f;

    [Header("Raygun Settings")]
    [SerializeField] private float DeathRayBeamMaxRange = 200f;
    [SerializeField] private float RaygunBroadBeamCastAngle = 1f;
    [SerializeField] private int RaygunBroadBeamRaycastCount = 9;
    [SerializeField] private GameObject CollisionExplosionPrefab;

    [Header("Aim Assist")]
    [SerializeField] public float AimAssistAngle = 38f;
    [SerializeField] public float AimAssistMaxRange = 35f;
    private bool AimAssistEnabled = true;

    [Header("Lock-On")]
    [SerializeField] private float GunTurnTime = 0.2f;
    [SerializeField] private float LockOnBeamDuration = 0.66f;
    [SerializeField] private LockOnReticle reticleObject;
    [SerializeField] private AimAssistReticle aimAssistReticleObject;
    [SerializeField] private float LockOnThreshold = 0.3f;
    [SerializeField] private float LockOnBaseTime = 1.5f;
    [SerializeField] private AudioClip _lockOnEnableSound;
    [SerializeField] private AudioClip _lockOnReadySound;
    [SerializeField] private AudioClip _lockOnStartSound;
    [SerializeField] private AudioClip _lockOnCancelSound;
    [SerializeField] private GameObject _lockOnGlyphPrefab;
    [SerializeField] private float LockOnCameraShakeDuration = 0.5f;
    [SerializeField] private float LockOnCameraShakeIntensity = 1f;

    [Header("Misc")]
    [SerializeField] private AttackTypes AttackType = AttackTypes.Standard;

    // --- Public Fields ---
    public bool ShouldTargetDissolve = false;
    public uint LaserType = 0;

    // --- Private Fields ---
    private float WeaponCooldown = 0.0f;
    private float GattlingGunCurrentTime = 0.0f;
    private AudioSource weaponSoundSource;
    private LineRenderer RaygunLine;
    private List<BaseAI> eligibleTargets;
    private Volume lockOnVolume;

    // Lock-On State
    private float lockOnTime = 0.0f;
    private float LockOnProgress = 0f;
    private bool lockOnStarted = false;
    private bool LockOnReady = false;
    private GameObject LockOnTarget = null;
    private bool LockOnEnabled = false;
    private bool hasPlayedLockOnReadySound = false;

    public enum AttackTypes
    {
        Standard,
        HeavyOnly,
        Raycast,
        BeamOnly
    }

    // --- Unity Methods ---

    void Awake()
    {
        RaygunLine = GetComponent<LineRenderer>();
        eligibleTargets = new List<BaseAI>();
    }

    void Start()
    {
        RaygunLine.enabled = false;
        weaponSoundSource = Player._PropController.getGeneralAudioSource;
        AttackType = Player._PropController.getAttackType;
        lockOnVolume = GetComponent<Volume>();
    }

    void Update()
    {
        if (GameManager.IsGamePaused)
            return;

        UpdateWeaponCooldown();
        HandleInput();
        UpdateLockOnState();
        UpdateAimAssistReticle();
    }

    void FixedUpdate()
    {
        if (!GameManager.IsGamePaused)
        {
            UpdateAimAssistTargets();
        }
    }

    // --- Input Handling ---

    private void HandleInput()
    {
        if (Input.GetButton("Fire1"))
        {
            HandlePrimaryFire();
        }
        else
        {
            GattlingGunCurrentTime = 0.0f;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            HandlePrimaryFireRelease();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            EnableLockOn();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            DisableLockOn();
        }
    }

    private void HandlePrimaryFire()
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
                    UpdateLockOnTimer();
                }
                else
                {
                    if (AimAssistEnabled && eligibleTargets.Count > 0)
                    {
                        AimAtTarget(eligibleTargets.First().transform.position);
                    }
                    else
                    {
                        Player._PropController.OnFire();
                        HandleGunStick();
                    }
                }
            }
        }
        else
        {
            if (AttackType != AttackTypes.HeavyOnly)
            {
                PlungerQuickAttack();
            }
            else
            {
                Player._PropController.OnFire();
                HandleGunStick();
            }
        }
    }

    private void HandlePrimaryFireRelease()
    {
        if (LockOnEnabled)
        {
            HandleLockOnFireRelease();
        }
        ResetLockOnState();
    }

    // --- Weapon Cooldown ---

    private void UpdateWeaponCooldown()
    {
        if (WeaponCooldown > 0)
        {
            WeaponCooldown -= Time.deltaTime;
        }
    }

    // --- Aim Assist ---

    private void UpdateAimAssistTargets()
    {
        RaycastHit hitTarget;
        int beamCount = (int)AimAssistAngle;
        float horizontalStartAngle = -AimAssistAngle / 2f;
        float horizontalAngleIncrement = AimAssistAngle / (float)(beamCount - 1);

        var hitList = new List<BaseAI>();

        for (int x = 0; x < beamCount; x++)
        {
            float currentHorizontalAngle = horizontalStartAngle + x * horizontalAngleIncrement;
            Vector3 rayDirection = Quaternion.Euler(0, currentHorizontalAngle, 0) * Player._PropController.getGunStickObject.transform.forward;
            Ray ray = new Ray(Player._PropController.getGunStickObject.transform.position, rayDirection);

            if (Physics.Raycast(ray, out hitTarget, AimAssistMaxRange))
            {
                var hitBaseAi = hitTarget.collider.gameObject.GetComponent<BaseAI>();
                if (hitBaseAi && !hitList.Contains(hitBaseAi))
                {
                    hitList.Add(hitBaseAi);
                }
            }
        }

        eligibleTargets.RemoveAll(a => !hitList.Contains(a));
        eligibleTargets.AddRange(hitList.Except(eligibleTargets));
    }

    private void UpdateAimAssistReticle()
    {
        if (AimAssistEnabled)
        {
            if (eligibleTargets.Count > 0)
            {
                aimAssistReticleObject.SetLockOnTarget(eligibleTargets.First().gameObject);
            }
            else
            {
                aimAssistReticleObject.ClearLockOnTarget();
            }
        }
    }

    private void AimAtTarget(Vector3 targetLocation)
    {
        targetLocation.y += 1;
        GetComponent<GunStickAimController>().AimGunstickTowardsQuickFire(targetLocation);
    }

    // --- Lock-On ---

    private void EnableLockOn()
    {
        LockOnEnabled = true;
        weaponSoundSource.PlayOneShot(_lockOnEnableSound);
        LockOnProgress = 0f;
        LockOnReady = false;
    }

    private void DisableLockOn()
    {
        LockOnEnabled = false;
        weaponSoundSource.PlayOneShot(_lockOnCancelSound);
        LockOnProgress = 0f;
        LockOnReady = false;
        LockOnTarget = null;
    }

    private void UpdateLockOnTimer()
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

    private void UpdateLockOnState()
    {
        lockOnVolume.enabled = LockOnEnabled;

        if (LockOnEnabled && Input.GetButtonDown("Fire1"))
        {
            TryStartLockOn();
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
                    hasPlayedLockOnReadySound = true;
                }
            }
            else
            {
                LockOnReady = false;
                reticleObject.LockOnValue = LockOnProgress / LockOnBaseTime;
            }
        }
        else
        {
            LockOnReady = false;
            reticleObject.LockOnValue = 0;
        }
    }

    private void TryStartLockOn()
    {
        lockOnStarted = false;
        if (AimAssistEnabled && eligibleTargets.Count > 0)
        {
            lockOnStarted = true;
            LockOnTarget = eligibleTargets.First().gameObject;
            LockOnProgress = 0f;
            reticleObject.SetLockOnTarget(LockOnTarget);
        }
        else
        {
            LockOnProgress = 0f;
            lockOnTime = 0.0f;
            lockOnStarted = false;
            LockOnTarget = null;
        }
    }

    private void HandleLockOnFireRelease()
    {
        if (LockOnReady && LockOnTarget != null)
        {
            if (GetComponent<InventoryController>().EquippedItems.Select(a => a.ItemTitle).Contains("Gun"))
            {
                Debug.Log("Fire at locked on target, which is a " + LockOnTarget.name);
                var targetPosition = LockOnTarget.transform.position;
                targetPosition.y++;
                GetComponent<GunStickAimController>().AimGunstickTowards(targetPosition, GunTurnTime, LockOnBeamDuration);
            }
            else
            {
                Debug.Log("Locked onto target with no gun, begin plunger attack if in range");
                if (Vector3.Distance(this.transform.position, LockOnTarget.transform.position) < PlungerRange)
                {
                    StartCoroutine(PlungerSlowAttack(LockOnTarget));
                }
            }
        }
        if (lockOnTime < LockOnThreshold)
        {
            weaponSoundSource.PlayOneShot(_lockOnCancelSound);
            Player._PropController.OnFire();
            HandleGunStick();
        }
    }

    private void ResetLockOnState()
    {
        LockOnReady = false;
        lockOnStarted = false;
        lockOnTime = 0.0f;
        LockOnProgress = 0f;
        LockOnTarget = null;
        hasPlayedLockOnReadySound = false;
        reticleObject.ClearLockOnTarget();
    }

    // --- Weapon Logic ---

    private void HandleGattlingGunActions()
    {
        if (GattlingGunCurrentTime == 0.0f)
            weaponSoundSource.PlayOneShot(GattlingGunWindupSound);

        if (GattlingGunCurrentTime < GattlingGunWindupTime)
        {
            GattlingGunCurrentTime += Time.deltaTime;
            return;
        }

        if (WeaponCooldown > 0)
            return;

        Player._PropController.OnRapidFire();
        FireGunStick(LaserType);
    }

    public void HandleGunStick()
    {
        switch (AttackType)
        {
            case AttackTypes.Standard:
            case AttackTypes.HeavyOnly:
                FireGunStick(LaserType);
                break;
            case AttackTypes.Raycast:
                HandleGunStickBroadBeam(LaserType);
                break;
            case AttackTypes.BeamOnly:
                HandleGunStickBeam();
                break;
        }
        GetComponentInChildren<PropController>()?.OnFire();
    }

    public void HandleGunStickBroadBeam(uint AttackStrength)
    {
        if (WeaponCooldown > 0)
            return;

        WeaponCooldown = GunStickDefaultFireRate;
        RaycastHit hitTarget;
        float furthestDistance = 0f;

        float horizontalStartAngle = -RaygunBroadBeamCastAngle / 2f;
        float horizontalAngleIncrement = RaygunBroadBeamCastAngle / (float)(RaygunBroadBeamRaycastCount - 1);

        for (int x = 0; x < RaygunBroadBeamRaycastCount; x++)
        {
            float currentHorizontalAngle = horizontalStartAngle + x * horizontalAngleIncrement;
            Vector3 rayDirection = Quaternion.Euler(0, currentHorizontalAngle, 0) *
                                   Player._PropController.getGunStickObject.transform.forward;
            Ray ray = new Ray(Player._PropController.getGunStickObject.transform.position, rayDirection);

            if (Physics.Raycast(ray, out hitTarget, DeathRayBeamMaxRange))
            {
                var baseAI = hitTarget.collider.GetComponent<BaseAI>();
                var damageable = hitTarget.collider.GetComponent<DamageableComponent>();
                if (baseAI || damageable)
                {
                    Debug.DrawRay(ray.origin, ray.direction * DeathRayBeamMaxRange, Color.red, 10f);
                    if (baseAI)
                    {
                        Player._PropController.PlayHitSoundatPoint(hitTarget.transform.position);
                        baseAI.Damage(new DamageInfo(50 + (50 * LaserType) * Player._PropController.getDamageMultiplier, gameObject, DamageType.DeathRay, hitTarget.transform));
                    }
                    if (damageable)
                    {
                        Player._PropController.PlayHitSoundatPoint(hitTarget.transform.position);
                        damageable.Damage(new DamageInfo(50 + (50 * LaserType) * Player._PropController.getDamageMultiplier, gameObject, DamageType.DeathRay, hitTarget.transform));
                    }
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * DeathRayBeamMaxRange, Color.white, 10f);
                }
                float dist = Vector3.Distance(gameObject.transform.position, hitTarget.collider.transform.position);
                if (dist > furthestDistance)
                {
                    furthestDistance = dist;
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * DeathRayBeamMaxRange, Color.white, 10f);
            }
        }

        Vector3 fwd = Player._PropController.getGunStickObject.transform.TransformDirection(Vector3.forward);
        RaygunLine.SetPosition(0, Player._PropController.getGunStickObject.transform.position);
        RaygunLine.endWidth = (RaygunBroadBeamCastAngle * 2) * (furthestDistance / DeathRayBeamMaxRange);
        RaygunLine.SetPosition(1, Player._PropController.getGunStickObject.transform.position + fwd * (furthestDistance == 0f ? DeathRayBeamMaxRange : furthestDistance));
        RaygunLine.enabled = true;
        StartCoroutine(HideDeathRayBeam(1f));
    }

    public void HandleGunStickBeam()
    {
        if (WeaponCooldown > 0)
            return;

        Player._PropController.OnFire();
        RaycastHit hit;
        Vector3 fwd = Player._PropController.getGunStickObject.transform.TransformDirection(Vector3.forward);
        RaygunLine.SetPosition(0, Player._PropController.getGunStickObject.transform.position);
        RaygunLine.endWidth = RaygunLine.startWidth;
        Debug.DrawRay(Player._PropController.getGunStickObject.transform.position, fwd, Color.green, 10f);

        if (Physics.Raycast(Player._PropController.getGunStickObject.transform.position, fwd, out hit, DeathRayBeamMaxRange))
        {
            RaygunLine.SetPosition(1, hit.point + (fwd * 0.33f));
            RaygunLine.enabled = true;
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit: " + hit.collider.gameObject.name);

            var npcAI = hit.collider.gameObject.GetComponent<BaseAI>();
            if (npcAI != null)
            {
                DamageInfo info = new DamageInfo(npcAI.MaxHealth, gameObject, DamageType.DeathRay, hit.transform);
                npcAI.Damage(info);
                AudioSource.PlayClipAtPoint(Player._PropController.RaygunBeamHit, hit.point, 1f);
            }
            Instantiate(CollisionExplosionPrefab, hit.point, Quaternion.Euler(fwd));
            CollisionExplosionPrefab.GetComponent<EnergyDischargeCollisionController>().SetLightType(1);
        }
        else
        {
            RaygunLine.SetPosition(1, Player._PropController.getGunStickObject.transform.position + fwd * DeathRayBeamMaxRange);
            RaygunLine.enabled = true;
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit nothing");
        }
        GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>().StartCameraShake(LockOnCameraShakeDuration, LockOnCameraShakeIntensity);

        WeaponCooldown = 1f;
    }

    private IEnumerator HideDeathRayBeam(float duration)
    {
        GetComponent<Movement>().MovementEnabled = false;
        StartCoroutine(Player._PropController.PlayExterminationLoop(duration));
        yield return new WaitForSeconds(duration);
        RaygunLine.enabled = false;
        RaygunLine.SetPosition(0, transform.position);
        RaygunLine.SetPosition(1, transform.position);
        GetComponent<Movement>().MovementEnabled = true;
    }

    private IEnumerator PlungerSlowAttack(GameObject target)
    {
        GetComponent<Movement>().MovementEnabled = false;
        Player._PropController.PlayAnimationClip(PropController.AnimationClips.PlungerSlowKill);
        Player._PropController.OnMeleeSlowKill();
        var targetObject = target.GetComponent<BaseAI>();
        var damage = new DamageInfo(targetObject.Health, this.gameObject, DamageType.Plunger);

        targetObject.PlungerSlowKill(damage);
        yield return new WaitForSeconds(3.2f);
        GetComponent<Movement>().MovementEnabled = true;
    }

    private void PlungerQuickAttack()
    {
        if (WeaponCooldown > 0)
            return;

        Player._PropController.PlayAnimationClip(PropController.AnimationClips.PlungerAttack);
        Player._PropController.OnMelee();
        WeaponCooldown = PlungerAttackRate;

        float startAngle = -PlungerAttackConeSpread / 2f;
        float angleIncrement = PlungerAttackConeSpread / (float)(PlungerAttackConeHitCount - 1);

        for (int i = 0; i < PlungerAttackConeHitCount; i++)
        {
            float currentAngle = startAngle + i * angleIncrement;
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * Player._PropController.getInteractionPoint.transform.forward;
            Ray ray = new Ray(Player._PropController.getInteractionPoint.transform.position, rayDirection);

            if (Physics.Raycast(ray, out RaycastHit hitTarget, PlungerAttackRange))
            {
                var npcAI = hitTarget.collider.gameObject.GetComponent<BaseAI>();
                var turretAI = hitTarget.collider.GetComponent<Turret>();
                var damageable = hitTarget.collider.gameObject.GetComponent<DamageableComponent>();
                var isBarrel = hitTarget.collider.gameObject.GetComponent<ExplosiveBarrel>() != null;

                if (npcAI != null)
                {
                    DamageInfo info = new DamageInfo(PlungerDamage, gameObject, DamageType.Plunger, hitTarget.transform);
                    npcAI.Damage(info);
                    var playerComponent = GetComponent<PlayerComponent>();
                    if (playerComponent.Health < playerComponent.MaxHealth)
                    {
                        playerComponent.Health += ((playerComponent.MaxHealth - playerComponent.Health) * PlungerHealthRechargePercentage);
                        playerComponent.Health = Mathf.Min(playerComponent.Health, playerComponent.MaxHealth);
                    }
                    return;
                }
                if (turretAI != null)
                {
                    DamageInfo info = new DamageInfo(PlungerDamage * 4, gameObject, DamageType.Plunger, hitTarget.transform);
                    turretAI.Damage(info);
                    return;
                }
                else if (damageable != null && !isBarrel)
                {
                    DamageInfo info = new DamageInfo(PlungerDamage, gameObject, DamageType.Plunger, hitTarget.transform);
                    damageable.Damage(info);
                    return;
                }
            }
        }
        Debug.Log("Plunger hit nothing");
    }

    private void FireGunStick(uint AttackStrength)
    {
        if (WeaponCooldown > 0)
            return;

        Player._PropController.PlaySoundClip(PropController.SoundClips.RaygunFire);
        WeaponCooldown = EnableGattlingGun ? GunStickDefaultFireRate * GattlingGunFireRateModifier : GunStickDefaultFireRate;
        GameObject go = Instantiate(Player._PropController.getRayGunDischargePrefab, Player._PropController.getGunStickObject.transform.position, Player._PropController.getGunStickObject.transform.rotation);
        go.transform.Rotate(0.0f, 0, 0.0f, Space.Self);
        go.GetComponent<EnergyDischargeController>().SetData(AttackStrength, ShouldTargetDissolve);
        Destroy(go, 3f);
    }
}