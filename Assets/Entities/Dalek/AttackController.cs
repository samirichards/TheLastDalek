using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class AttackController : MonoBehaviour
{
    public bool GunStickEnabled = true;
    [SerializeField] float GunStickDefaultFireRate = 0.5f;
    [SerializeField] private float GattlingGunFireRateModifier = 0.2f;
    [SerializeField] float PlungerAttackRate = 1f;
    private float WeaponCooldown = 0.0f;
    public float PlungerAttackRange = 1.5f;
    public float PlungerDamage = 35f;
    public float PlungerHealthRechargePercentage = 0.05f;
    public float PlungerAttackConeSpread = 40;
    public float PlungerAttackConeHitCount = 5;
    [SerializeField] private float DeathRayBeamMaxRange = 200f;
    [SerializeField] public GameObject GattlingGunModel;
    [SerializeField] public AudioClip GattlingGunWindupSound;
    [SerializeField] public float GattlingGunWindupTime = 0.33f;
    [SerializeField] public bool EnableGattlingGun;
    private float GattlingGunCurrentTime = 0.0f;
    private AudioSource weaponSoundSource;
    public uint LaserType = 0;
    private LineRenderer RaygunLine;
    [SerializeField]private float RaygunBroadBeamCastAngle = 1f;
    [SerializeField]private int RaygunBroadBeamRaycastCount = 9;

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
    [SerializeField] private AttackTypes AttackType = AttackTypes.Standard;

    private Volume lockOnVolume;

    public enum AttackTypes
    {
        Standard,
        HeavyOnly,
        Raycast,
        BeamOnly
    }

    void Awake()
    {
        RaygunLine = GetComponent<LineRenderer>();
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
            lockOnVolume.enabled = true;
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
            lockOnVolume.enabled = false;
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

        }
    }

    void HandleGattlingGunActions()
    {
        //weaponSoundSource.volume = 0.66f;

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
        switch (AttackType)
        {
            case AttackTypes.Standard:
                FireGunStick(LaserType);
                break;
            case AttackTypes.Raycast:
                HandleGunStickBroadBeam(LaserType);
                break;
            case AttackTypes.HeavyOnly:
                Debug.Log("Heavy only attack type not implemented");
                break;
            case AttackTypes.BeamOnly:
                HandleGunStickBeam();
                break;
        }
    }

    public void HandleGunStickBroadBeam(uint AttackStrength)
    {
        RaycastHit HitTarget;
        float furthestDistance = 0f;
        if (WeaponCooldown > 0)
        {
            return;
        }
        WeaponCooldown = GunStickDefaultFireRate;

        float verticalStartAngle = -RaygunBroadBeamCastAngle / 2f;
        float horizontalStartAngle = -RaygunBroadBeamCastAngle / 2f;

        float horizontalAngleIncrement = RaygunBroadBeamCastAngle / (float)(RaygunBroadBeamRaycastCount - 1);
        float verticalAngleIncrement = RaygunBroadBeamCastAngle / (float)(RaygunBroadBeamRaycastCount - 1);

        for (int y = 0; y < RaygunBroadBeamRaycastCount; y++)
        {

        }

        for (int x = 0; x < RaygunBroadBeamRaycastCount; x++)
        {
            float currentHorizontalAngle = horizontalStartAngle + x * horizontalAngleIncrement;
            //float currentVerticalAngle = verticalStartAngle + y * verticalAngleIncrement;
            float currentVerticalAngle = 0f;

            Vector3 rayDirection = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0) *
                                   Player._PropController.getGunStickObject.transform.forward;
            Ray ray = new Ray(Player._PropController.getGunStickObject.transform.position, rayDirection);

            if (Physics.Raycast(ray, out HitTarget, DeathRayBeamMaxRange))
            {

                if (HitTarget.collider.gameObject.GetComponent<BaseAI>() || HitTarget.collider.gameObject.GetComponent<DamageableComponent>())
                {
                    Debug.DrawRay(ray.origin, ray.direction * DeathRayBeamMaxRange, Color.red, 10f);
                    if (HitTarget.collider.GetComponent<BaseAI>())
                    {
                        Player._PropController.PlayHitSoundatPoint(HitTarget.transform.position);
                        HitTarget.collider.GetComponent<BaseAI>().Damage(new DamageInfo(50 + (50 * LaserType) * Player._PropController.getDamageMultiplier, gameObject, DamageType.DeathRay));
                    }
                    if (HitTarget.collider.GetComponent<DamageableComponent>())
                    {
                        Player._PropController.PlayHitSoundatPoint(HitTarget.transform.position);
                        HitTarget.collider.GetComponent<DamageableComponent>().Damage(new DamageInfo(50 + (50 * LaserType) * Player._PropController.getDamageMultiplier, gameObject, DamageType.DeathRay));
                    }
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * DeathRayBeamMaxRange, Color.white, 10f);
                }
                if (Vector3.Distance(gameObject.transform.position, HitTarget.collider.transform.position) > furthestDistance)
                {
                    furthestDistance = Vector3.Distance(gameObject.transform.position, HitTarget.collider.transform.position);
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
        if (furthestDistance == 0f)
        {
            RaygunLine.SetPosition(1, Player._PropController.getGunStickObject.transform.position + fwd * DeathRayBeamMaxRange);
        }
        else
        {
            RaygunLine.SetPosition(1, Player._PropController.getGunStickObject.transform.position + fwd * furthestDistance);
        }
        RaygunLine.enabled = true;
        StartCoroutine(HideDeathRayBeam(1f));
    }

    public void HandleGunStickBeam()
    {
        if (WeaponCooldown > 0)
        {
            return;
        }
        
        //weaponSoundSource.volume = 0.5f;
        RaycastHit hit;
        Vector3 fwd = Player._PropController.getGunStickObject.transform.TransformDirection(Vector3.forward);
        RaygunLine.SetPosition(0, Player._PropController.getGunStickObject.transform.position);
        RaygunLine.endWidth = RaygunLine.startWidth;
        Debug.DrawRay(Player._PropController.getGunStickObject.transform.position, fwd, Color.green, 10f);
        if (Physics.Raycast(Player._PropController.getGunStickObject.transform.position, fwd, out hit, DeathRayBeamMaxRange))
        {
            RaygunLine.SetPosition(1, hit.point);
            RaygunLine.enabled = true;
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit: " + hit.collider.gameObject.name);
            var npcAI = hit.collider.gameObject.GetComponent<BaseAI>();
            if (npcAI != null)
            {
                DamageInfo info = new DamageInfo(npcAI.MaxHealth, gameObject, DamageType.DeathRay);
                npcAI.Damage(info);
                AudioSource.PlayClipAtPoint(Player._PropController.RaygunBeamHit, hit.point, 1f);
            }

        }
        else
        {
            RaygunLine.SetPosition(1, Player._PropController.getGunStickObject.transform.position + fwd*DeathRayBeamMaxRange);
            RaygunLine.enabled = true;
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit nothing");
        }

        WeaponCooldown = 1f;
    }

    IEnumerator HideDeathRayBeam(float duration)
    {
        GetComponent<Movement>().MovementEnabled = false;
        StartCoroutine(Player._PropController.PlayExterminationLoop(duration));
        yield return new WaitForSeconds(duration);
        RaygunLine.enabled = false;
        RaygunLine.SetPosition(0, transform.position);
        RaygunLine.SetPosition(1, transform.position);
        GetComponent<Movement>().MovementEnabled = true;
    }

    private void PlungerQuickAttack()
    {
        bool hasHitSomething = false;
        RaycastHit HitTarget;
        if (WeaponCooldown > 0)
        {
            return;
        }
        Player._PropController.PlayAnimationClip(PropController.AnimationClips.PlungerAttack);
        WeaponCooldown = PlungerAttackRate;


        float startAngle = - PlungerAttackConeSpread / 2f;
        float angleIncrement = PlungerAttackConeSpread / (float)(PlungerAttackConeHitCount - 1);

        for (int i = 0; i < PlungerAttackConeHitCount; i++)
        {
            // Calculate the current angle
            float currentAngle = startAngle + i * angleIncrement;

            // Calculate the direction of the ray
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * Player._PropController.getInteractionPoint.transform.forward;

            // Fire the raycast
            Ray ray = new Ray(Player._PropController.getInteractionPoint.transform.position, rayDirection);
            if (Physics.Raycast(ray, out HitTarget, PlungerAttackRange))
            {
                if (HitTarget.collider.gameObject.GetComponent<BaseAI>() || HitTarget.collider.gameObject.GetComponent<DamageableComponent>())
                {
                    Debug.DrawRay(ray.origin, ray.direction * PlungerAttackRange, Color.red, 5f);
                    hasHitSomething = true;
                    Debug.Log("Plunger hit: " + HitTarget.collider.gameObject.name);
                    var npcAI = HitTarget.collider.gameObject.GetComponent<BaseAI>();
                    var turretAI = HitTarget.collider.GetComponent<Turret>();
                    if (npcAI != null)
                    {
                        DamageInfo info = new DamageInfo(PlungerDamage, gameObject, DamageType.Plunger);
                        npcAI.Damage(info);
                        var playerComponent = GetComponent<PlayerComponent>();
                        if (playerComponent.Health < playerComponent.MaxHealth)
                        {
                            playerComponent.Health += ((playerComponent.MaxHealth - playerComponent.Health) * PlungerHealthRechargePercentage);
                            if (playerComponent.Health > playerComponent.MaxHealth)
                            {
                                playerComponent.Health = playerComponent.MaxHealth;
                            }
                        }
                        return;
                    }
                    if (turretAI != null)
                    {
                        DamageInfo info = new DamageInfo(PlungerDamage * 4, gameObject, DamageType.Plunger);
                        turretAI.Damage(info);
                        return;
                    }
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * PlungerAttackRange, Color.white, 5f);
                }
            }
        }
        Debug.Log("Plunger hit nothing");
    }

    private void FireGunStick(uint AttackStrength)
    {
        if (WeaponCooldown > 0)
        {
            return;
        }
        Player._PropController.PlaySoundClip(PropController.SoundClips.RaygunFire);
        WeaponCooldown = EnableGattlingGun ? GunStickDefaultFireRate * GattlingGunFireRateModifier : GunStickDefaultFireRate;
        GameObject go = Instantiate(Player._PropController.getRayGunDischargePrefab, Player._PropController.getGunStickObject.transform.position, Player._PropController.getGunStickObject.transform.rotation);
        go.transform.Rotate(0.0f, 0, 0.0f, Space.Self);
        go.GetComponent<EnergyDischargeController>().SetData(AttackStrength);
        GameObject.Destroy(go, 3f);
    }
}