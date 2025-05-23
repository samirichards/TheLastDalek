using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class AttackController : MonoBehaviour
{
    /// <summary>
    /// Rework all of this shit tbh, it's so ass
    /// It should have some form of mild aim assist, on top of the lock on stuff
    /// How weapons are fired rn is such a tangled mess of nested ifs
    /// </summary>
    public bool GunStickEnabled = true;
    [SerializeField] public float GunStickDefaultFireRate = 0.5f;
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
    [SerializeField] private GameObject CollisionExplosionPrefab;
    [SerializeField] public float AimAssistAngle = 38f;
    [SerializeField] public float AimAssistMaxRange = 35f;
    private bool AimAssistEnabled = true;
    [SerializeField] private float PlungerRange = 20f;


    public bool ShouldTargetDissolve = false;

    [SerializeField] private float GunTurnTime = 0.2f;
    [SerializeField] private float LockOnBeamDuration = 0.66f;
    [SerializeField] private LockOnReticle reticleObject;
    [SerializeField] private AimAssistReticle aimAssistReticleObject;
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
    [SerializeField] private float LockOnCameraShakeDuration = 0.5f;
    [SerializeField] private float LockOnCameraShakeIntensity = 1f;
    [SerializeField] private AttackTypes AttackType = AttackTypes.Standard;

    private List<BaseAI> eligableTargets;


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
        eligableTargets = new List<BaseAI>();
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
                        if (AimAssistEnabled && eligableTargets.Count > 0)
                        {
                            var targetLocation = eligableTargets.First().transform.position;
                            targetLocation.y += 1;
                            //Without this the dalek aims at their feet
                            GetComponent<GunStickAimController>().AimGunstickTowardsQuickFire(targetLocation);
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
                //RaycastHit hit;
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Ditch the whole mouse thing, it wouldn't translate well to controller, so just use the same target as aim assist
                if (AimAssistEnabled)
                {
                    if (eligableTargets.Count > 0)
                    {
                        lockOnStarted = true;
                        LockOnTarget = eligableTargets.First().gameObject;
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
                else
                {
                    LockOnProgress = 0f;
                    lockOnTime = 0.0f;
                    lockOnStarted = false;
                    LockOnTarget = null;
                }

                //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                //{
                //    if (hit.collider.CompareTag("NPC")) // Replace "NPC" with your NPC tag
                //    {
                //        lockOnStarted = true;
                //        LockOnTarget = hit.collider.gameObject;
                //        LockOnProgress = 0f;
                //        reticleObject.SetLockOnTarget(LockOnTarget);
                //    }
                //    else
                //    {
                //        // Handle the case when the mouse is not over an NPC
                //        LockOnProgress = 0f;
                //        lockOnTime = 0.0f;
                //        lockOnStarted = false;
                //        LockOnTarget = null;
                //    }
                //}
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
                    if (this.GetComponent<InventoryController>().EquippedItems.Select(a=>a.ItemTitle).Contains("Gun"))
                    {
                        Debug.Log("Fire at locked on target, which is a " + LockOnTarget.name);
                        var targetPosition = LockOnTarget.transform.position;
                        targetPosition.y++;
                        GetComponent<GunStickAimController>().AimGunstickTowards(targetPosition, GunTurnTime, LockOnBeamDuration);
                    }
                    else
                    {
                        Debug.Log("Lockeed onto target with no gun, begin plunger attack if in range");
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

        //Just bolting on the partial lock on stuff here until I can be bothered to change it properly
        //Should raycast out continuously, and try to aim at the nearest/closest to the center line target within a cone of about 15 degrees
        //Should show targeting reticule on who it is aiming at
        if (AimAssistEnabled)
        {
            if (eligableTargets.Count > 0)
            {
                //Should be sorted by distance but isn't rn
                aimAssistReticleObject.SetLockOnTarget(eligableTargets.First().gameObject);
            }
            else
            {
                aimAssistReticleObject.ClearLockOnTarget();
            }
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.IsGamePaused)
        {

        }

        RaycastHit HitTarget;
        int beamCount = (int)AimAssistAngle;

        float verticalStartAngle = -AimAssistAngle / 8f;
        float horizontalStartAngle = -AimAssistAngle / 2f;

        float horizontalAngleIncrement = AimAssistAngle / (float)(beamCount - 1);
        float verticalAngleIncrement = (AimAssistAngle / (float)(beamCount - 1))*4;
        var hitList = new List<BaseAI>();

        //for (int y = 0; y < (beamCount / 8); y++)
        //{
            for (int x = 0; x < beamCount; x++)
            {
                float currentHorizontalAngle = horizontalStartAngle + x * horizontalAngleIncrement;
                //float currentVerticalAngle = verticalStartAngle + y * verticalAngleIncrement;
                float currentVerticalAngle = 0; //verticalStartAngle + y * verticalAngleIncrement;

                Vector3 rayDirection = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0) * Player._PropController.getGunStickObject.transform.forward;
                Ray ray = new Ray(Player._PropController.getGunStickObject.transform.position, rayDirection);

                if (Physics.Raycast(ray, out HitTarget, AimAssistMaxRange))
                {
                    var hitBaseAi = HitTarget.collider.gameObject.GetComponent<BaseAI>();
                    if (hitBaseAi)
                    {
                        //Debug.DrawRay(ray.origin, ray.direction * AimAssistMaxRange, Color.red, 0.02f);
                        //Add or remove it from the target list depending on if it's already there or if there is something close
                        if (!hitList.Contains(hitBaseAi))
                        {
                            hitList.Add(hitBaseAi);
                        }
                    }
                    else
                    {
                        //Debug.DrawRay(ray.origin, ray.direction * AimAssistMaxRange, Color.green, 0.02f);
                    }
                }
                else
                {
                    //Debug.DrawRay(ray.origin, ray.direction * AimAssistMaxRange, Color.green, 0.02f);
                }

            }
       //}
        eligableTargets.Where(a => !hitList.Contains(a));
        eligableTargets.RemoveAll(a => !hitList.Contains(a));
        eligableTargets.AddRange(hitList.Distinct());
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
        Player._PropController.OnRapidFire();
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
                FireGunStick(LaserType);
                break;
            case AttackTypes.BeamOnly:
                HandleGunStickBeam();
                break;
        }
        GetComponentInChildren<PropController>()?.OnFire();
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
        //Unused behavior to make beam spread vertically as well as horizontally

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
        Player._PropController.OnFire();

        //weaponSoundSource.volume = 0.5f;
        RaycastHit hit;
        Vector3 fwd = Player._PropController.getGunStickObject.transform.TransformDirection(Vector3.forward);
        RaygunLine.SetPosition(0, Player._PropController.getGunStickObject.transform.position);
        RaygunLine.endWidth = RaygunLine.startWidth;
        Debug.DrawRay(Player._PropController.getGunStickObject.transform.position, fwd, Color.green, 10f);
        if (Physics.Raycast(Player._PropController.getGunStickObject.transform.position, fwd, out hit, DeathRayBeamMaxRange))
        {
            //Continue the beam past the collider a little bit to look like its actually hitting the model itself
            RaygunLine.SetPosition(1, hit.point + (fwd * 0.33f));
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
            Instantiate(CollisionExplosionPrefab, hit.point, Quaternion.Euler(fwd));
            CollisionExplosionPrefab.GetComponent<EnergyDischargeCollisionController>().SetLightType(1);

        }
        else
        {
            RaygunLine.SetPosition(1, Player._PropController.getGunStickObject.transform.position + fwd*DeathRayBeamMaxRange);
            RaygunLine.enabled = true;
            StartCoroutine(HideDeathRayBeam(LockOnBeamDuration));
            Debug.Log("Death ray beam hit nothing");
        }
        GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>().StartCameraShake(LockOnCameraShakeDuration, LockOnCameraShakeIntensity);

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
        bool hasHitSomething = false;
        RaycastHit HitTarget;
        if (WeaponCooldown > 0)
        {
            return;
        }
        Player._PropController.PlayAnimationClip(PropController.AnimationClips.PlungerAttack);
        Player._PropController.OnMelee();
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
                        DamageInfo info = new DamageInfo(PlungerDamage, gameObject, DamageType.Plunger, HitTarget.transform);
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
                        DamageInfo info = new DamageInfo(PlungerDamage * 4, gameObject, DamageType.Plunger, HitTarget.transform);
                        turretAI.Damage(info);
                        return;
                    }
                    else if(HitTarget.collider.gameObject.GetComponent<DamageableComponent>() && HitTarget.collider.gameObject.GetComponent<ExplosiveBarrel>() == null)
                    {
                        DamageInfo info = new DamageInfo(PlungerDamage, gameObject, DamageType.Plunger, HitTarget.transform);
                        HitTarget.collider.gameObject.GetComponent<DamageableComponent>().Damage(info);
                        return;
                    }
                    //Extremely messy but I really can't be arsed to untangle it and make it neat, so I'm just adding to the slop pile by making it do a check for if it isn't a barrel
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
        go.GetComponent<EnergyDischargeController>().SetData(AttackStrength, ShouldTargetDissolve);
        GameObject.Destroy(go, 3f);
    }
}