using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerComponent : MonoBehaviour
{
    public BoxCollider PickupCollider;

    public float Health;
    public float MaxHealth = 100f;
    public GameObject ShieldObject;
    private AudioSource audioSource;
    public AudioClip[] BulletImpactSounds;
    public AudioClip[] SteamReleaseSounds;
    public AudioClip DamageClip;
    public AudioClip DeathClip;
    public bool IsAlive = true;
    private ShieldManager shieldManager;
    public GameObject HealthBar;
    public GameObject ShieldBar;
    [SerializeField] private bool CanHackStuff = false;
    private bool _canSeeStuff = false;
    public bool CanSeeHiddenObjects
    {
        get
        {
            return _canSeeStuff;
        }
        set
        {
            _canSeeStuff = value;
            StartCoroutine(SetHiddenObjectVisibility(_canSeeStuff));
        }
    }

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

    private void OnValidate()
    {
        StartCoroutine(SetHiddenObjectVisibility(CanSeeHiddenObjects));
    }

    void Start()
    {
        Health = MaxHealth;
        GetComponent<Animator>().SetBool("IsAlive", true);
    }

    void FixedUpdate()
    {
        //healthBarSlider.value = Health * 0.01f;
        //shieldBarSlider.value = shieldManager.ShieldHealth * 0.01f;
        //ShieldBar.SetActive(shieldManager.ShieldEnabled);
    }

    public void Damage(DamageInfo _damageInfo)
    {
        Health -= _damageInfo.DamageValue;
        audioSource.PlayOneShot(DamageClip);
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
        audioSource.PlayOneShot(DeathClip);
        GetComponent<Animator>().Play(Animator.StringToHash("Death"), -1, 0);
        //GetComponent<Animator>().SetBool("IsAlive", false);
        GetComponent<Movement>().MovementEnabled = false;
        GetComponent<AttackController>().enabled = false;
        GameManager.GetLevelTransitionManager().ReloadScene(3);
        //Implement death behaviour 
    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        shieldManager = ShieldObject.GetComponent<ShieldManager>();
        //healthBarSlider = HealthBar.GetComponent<Slider>();
        //shieldBarSlider = ShieldBar.GetComponent<Slider>();
    }

    private void OnApplicationQuit()
    {

    }
}