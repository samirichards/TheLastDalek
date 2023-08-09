using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private float LockOnThreshold = 0.3f;
    public float lockOnTime = 0.0f;
    public bool lockOnStarted = false;
    public bool LockOnEnabled = false;

    private float inputTimer = 0.0f;

    private GameManager _gameManagerComponent;

    void Start()
    {
        weaponSoundSource = gameObject.AddComponent<AudioSource>();
        GattlingGunEmitter.SetActive(false);
        _gameManagerComponent = GetComponent<GameManager>();
    }

    void Update()
    {
        if (_gameManagerComponent.IsGamePaused)
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

        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Fire button let go of");
            if (GunStickEnabled)
            {
                if (LockOnEnabled)
                {
                    if (lockOnTime < LockOnThreshold)
                    {
                        HandleGunStick();
                        lockOnStarted = false;
                        lockOnTime = 0.0f;
                    }
                }
                if (lockOnTime < LockOnThreshold)
                {
                    HandleGunStick();
                    lockOnStarted = false;
                    lockOnTime = 0.0f;
                }
            }
            lockOnStarted = false;
            lockOnTime = 0.0f;
        }
    }

    void FixedUpdate()
    {
        if (!_gameManagerComponent.IsGamePaused)
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

        GattlingGunEmitter.SetActive(true);
        FireGunStick(LaserType);
        GattlingGunModel.transform.Rotate(GattlingGunSpinSpeed, 0, 0 * Time.deltaTime);
    }

    void HandleGunStick()
    {
        weaponSoundSource.volume = 0.8f;
        FireGunStick(LaserType);
        GattlingGunEmitter.SetActive(false);
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