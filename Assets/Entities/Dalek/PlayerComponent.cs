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

    [Header("CoreAbilities ---")] private bool _canSeeStuff = false;
    [SerializeField] private bool CanHackStuff = false;

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
        MaxHealth = MaxHealth * Player._PropController.getHealthMultiplier;
        Health = MaxHealth;
        GetComponent<Animator>().SetBool("IsAlive", true);
        ShieldHealth = ShieldMaxHealth;
    }

    void FixedUpdate()
    {
        //healthBarSlider.value = Health * 0.01f;
        //shieldBarSlider.value = shieldManager.ShieldHealth * 0.01f;
        //ShieldBar.SetActive(shieldManager.ShieldEnabled);
    }

    void Update()
    {
        ShieldEffective = ShieldHealth > 0 && ShieldEnabled;
        if (ShieldHealth < ShieldMaxHealth && ShieldRechargeDelayTimer <= 0)
        {
            ShieldHealth += ShieldRechargeRate * Time.deltaTime;
        }
        else
        {
            if (ShieldRechargeDelayTimer > 0)
            {
                ShieldRechargeDelayTimer -= Time.deltaTime;
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

    public void DamageShield(DamageInfo info)
    {

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
            //CharacterAnimator.Play(Animator.StringToHash("Damage"), -1, 0);
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