using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PlayerComponent : MonoBehaviour
{
    [Header("Health Stats ---")] public float Health;
    public float MaxHealth = 100f;
    public bool IsAlive = true;

    [Header("Core Abilities ---")] private bool _canSeeStuff = false;
    [SerializeField] private bool CanHackStuff = false;

    [Header("Damage Effects ---")]
    public GameObject SteamPrefab;

    public bool CanSeeHiddenObjects
    {
        get { return _canSeeStuff; }
        set
        {
            _canSeeStuff = value;
            StartCoroutine(SetHiddenObjectVisibility(_canSeeStuff));
        }
    }


    [Header("Shield Stats ---")] public float ShieldMaxHealth = 100f;
    public float ShieldHealth;
    public float ShieldRechargeRate = 6.66f;
    public float ShieldRechargeDelay = 3f;
    private float ShieldRechargeDelayTimer = 0.0f;
    private bool ShieldIsRecharging = false;
    public bool ShieldEnabled = false;
    public bool ShieldEffective = true;
    public int ShieldTier = 0;

    [Header("Misc ---")] public BoxCollider PickupCollider;
    private AudioSource audioSource;
    public AudioClip[] BulletImpactSounds;
    public AudioClip[] SteamReleaseSounds;

    public bool GetPrivileges()
    {
        return CanHackStuff;
    }

    public void SetPrivileges(bool b)
    {
        CanHackStuff = b;
    }

    private IEnumerator SetHiddenObjectVisibility(bool visible)
    {
        foreach (Mine _mine in Object.FindObjectsOfType<Mine>())
        {
            _mine.SetVisibility(visible);
        }

        //DO THE SAME FOR ENEMIES THAT CAN BE HIDDEN
        //I would do it now but I haven't gotten round to doing that so... here we are
        //TODO Add functionality to reveal enemies
        yield return null;
    }

    void Start()
    {
        MaxHealth *= Player._PropController.getHealthMultiplier;
        Health = MaxHealth;
        ShieldHealth = ShieldMaxHealth;
    }

    void Update()
    {
        ShieldEffective = ShieldHealth > 0 && ShieldEnabled;
        if (ShieldHealth < ShieldMaxHealth && ShieldRechargeDelayTimer <= 0)
        {
            ShieldHealth += ShieldRechargeRate * Time.deltaTime;
            ShieldIsRecharging = true;
        }
        else
        {
            ShieldIsRecharging = false;

            if (ShieldRechargeDelayTimer > 0)
            {
                ShieldRechargeDelayTimer -= Time.deltaTime;
            }
        }

        if (!GameManager.IsGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Player._PropController.PlaySoundClip(PropController.SoundClips.ExterminateVO);
            }
        }
    }

    /// <summary>
    /// Enables the shield
    /// 200 max charges with recharge rate of 13.333 per second with tier 1
    /// 100 max charges with recharge rate of 6.666 per seconds with tier 0
    /// </summary>

    public void ShieldSetActive()
    {
        if (ShieldTier > 0)
        {
            //Enhanced Shield behavior
            ShieldMaxHealth = 200;
            ShieldRechargeDelay = 2f;
            ShieldRechargeRate = 13.333f;
            //From 0 to 200, this gives an effective recharge time of 15 seconds
            ShieldEnabled = true;
            ShieldEffective = true;
        }
        else
        {
            //Default shield behavior
            ShieldMaxHealth = 100;
            ShieldRechargeDelay = 3f;
            ShieldRechargeRate = 6.666f;
            //From 0 to 100, this gives an effective recharge time of 15 seconds
            ShieldEnabled = true;
            ShieldEffective = true;
        }
    }

    public void ShieldSetInactive()
    {
        ShieldEnabled = false;
        ShieldEffective = false;
    }

    public void SetShieldDisabled()
    {
        ShieldEffective = false;
    }

    public void Damage(DamageInfo _damageInfo)
    {
        if (ShieldEffective)
        {
            ShieldRechargeDelayTimer = ShieldRechargeDelay;
            ShieldHealth -= _damageInfo.DamageValue;
            if (ShieldHealth < 0)
            {
                ShieldHealth = 0;
                SetShieldDisabled();
            }

            return;
        }

        Health -= _damageInfo.DamageValue;
        Player._PropController.PlaySoundClip(PropController.SoundClips.DamageSFX);
        if (_damageInfo.DamageType == DamageType.Bullet)
        {
            audioSource.PlayOneShot(BulletImpactSounds[Mathf.RoundToInt(Random.Range(0, BulletImpactSounds.Length))]);
            audioSource.PlayOneShot(SteamReleaseSounds[Mathf.RoundToInt(Random.Range(0, SteamReleaseSounds.Length))]);

            if (_damageInfo.ImpactLocation != null)
            {
                // Instantiate steam prefab at the exact position of the impact
                var steam = Instantiate(SteamPrefab, _damageInfo.ImpactLocation.position, Quaternion.identity);

                // Optional: Parent the steam to the player if needed
                steam.transform.SetParent(gameObject.transform);

                // Calculate the direction to mirror
                Vector3 incomingDirection = (_damageInfo.ImpactLocation.position - _damageInfo.DamageSource.transform.position).normalized;
                Vector3 mirroredDirection = -incomingDirection; // Reverse the direction

                // Set the rotation of the steam effect to face the mirrored direction
                steam.transform.rotation = Quaternion.LookRotation(mirroredDirection);

                // Apply additional rotation if needed (e.g., making the steam blow upward slightly)
                steam.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
            }
        }

        if (Health <= 0f && IsAlive)
        {
            Health = 0f;
            Die(_damageInfo);
        }
    }

    void Die(DamageInfo _damageInfo)
    {
        IsAlive = false;
        Player._PropController.PlaySoundClip(PropController.SoundClips.DeathVO);
        Player._PropController.PlayAnimationClip(PropController.AnimationClips.Death);
        //GetComponent<Animator>().SetBool("IsAlive", false);
        GetComponent<Movement>().MovementEnabled = false;
        GetComponent<AttackController>().enabled = false;
        GameManager.GetLevelTransitionManager().ReloadScene(3);
        //Implement death behaviour 
    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        //healthBarSlider = HealthBar.GetComponent<Slider>();
        //shieldBarSlider = ShieldBar.GetComponent<Slider>();
    }
}