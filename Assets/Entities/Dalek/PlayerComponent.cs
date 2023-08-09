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
    private bool IsAlive = true;
    private ShieldManager shieldManager;
    public GameObject HealthBar;
    public GameObject ShieldBar;

    void Start()
    {
        Health = MaxHealth;
    }

    void FixedUpdate()
    {
        //healthBarSlider.value = Health * 0.01f;
        //shieldBarSlider.value = shieldManager.ShieldHealth * 0.01f;
        //ShieldBar.SetActive(shieldManager.ShieldEnabled);
    }

    void OnCollisionEnter(Collision collision)
    {
        var item = collision.gameObject.GetComponent<GroundItem>();
        if (item)
        {
            if (GetComponent<InventoryManager>().AddItem(item._item))
            {
                GetComponent<GameManager>().ShowUpgradeScreen(item._item.ItemID, GetComponent<InventoryManager>().GetItems().First(a=> a.item.ItemTitle == item._item.ItemTitle).ItemTier);
                Destroy(collision.gameObject);
            }
        }
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
        GetComponent<InventoryManager>().ClearInventory();
    }
}