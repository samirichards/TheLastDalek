using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GuardAI : BaseAI
{
    [SerializeField] protected GameObject PistolObject;
    [SerializeField] protected GameObject RifleObject;
    [SerializeField] protected GameObject SpecialWeaponObject;
    [SerializeField] protected EquippedWeapon equippedWeapon;
    [SerializeField] protected GameObject BulletPrefab;

    [SerializeField] protected float PistolFireRate = 1.2f;
    [SerializeField] protected float RifleFireRate = 0.66666f;
    [SerializeField] protected float SpecialWeaponFireRate = 1.5f;
    [SerializeField] private AudioSource weaponSoundSource;

    [SerializeField] private AudioClip[] PistolShootSounds;
    [SerializeField] private AudioClip[] RifleShootSounds;
    [SerializeField] private AudioClip[] SpecialWeaponShootSounds;

    public float WeaponCooldown = 0.0f;
    public GameObject AttackTarget;
    public bool AttackInProgress = false;
    public override void SetDefaults()
    {
        npcType = NPCType.Hostile;
        Emotion = EmotionState.Normal;
        RifleObject.SetActive(false);
        SpecialWeaponObject.SetActive(false);
        PistolObject.SetActive(false);
        DalekTarget = GameObject.Find("Player");
        switch (equippedWeapon)
        {
            case EquippedWeapon.Rifle:
                RifleObject.SetActive(true);
                CharacterAnimator.SetBool("WeaponType", true);
                break;
            case EquippedWeapon.SpecialWeapon:
                SpecialWeaponObject.SetActive(true);
                CharacterAnimator.SetBool("WeaponType", false);
                break;
            case EquippedWeapon.Pistol:
                PistolObject.SetActive(true);
                CharacterAnimator.SetBool("WeaponType", false);
                break;
            default:
                PistolObject.SetActive(true);
                CharacterAnimator.SetBool("WeaponType", false);
                break;
        }
    }

    public override void CustomUpdateBehaviour()
    {
        if (WeaponCooldown > 0)
        {
            WeaponCooldown -= Time.deltaTime;
        }

        if (AttackInProgress && IsAlive)
        {
            switch (equippedWeapon)
            {
                case EquippedWeapon.Pistol:
                    HandlePistolShot();
                    break;
                case EquippedWeapon.Rifle:
                    HandleRifleShot();
                    break;
                case EquippedWeapon.SpecialWeapon:
                    HandleSpecialShot();
                    break;
                default:
                    HandlePistolShot();
                    break;
            }
        }

    }

    private void HandlePistolShot()
    {
        Debug.Log("Attempt Shot");
        if (WeaponCooldown > 0)
            return;

        weaponSoundSource.volume = 0.5f;
        weaponSoundSource.PlayOneShot(PistolShootSounds[Mathf.RoundToInt(Random.Range(0, PistolShootSounds.Length))]);
        CharacterAnimator.SetTrigger("Shoot");
        WeaponCooldown = PistolFireRate;

        GameObject go = Instantiate(BulletPrefab, PistolObject.transform.position, PistolObject.transform.rotation);
        go.transform.LookAt(new Vector3(AttackTarget.transform.position.x, AttackTarget.transform.position.y + 1, AttackTarget.transform.position.z));
        go.GetComponent<Bullet>().SetData(gameObject,6f);
        GameObject.Destroy(go, 3f);



    }
    private void HandleRifleShot()
    {
        Debug.Log("Attempt Shot");
        if (WeaponCooldown > 0)
            return;

        weaponSoundSource.volume = 0.5f;
        weaponSoundSource.PlayOneShot(RifleShootSounds[Mathf.RoundToInt(Random.Range(0, RifleShootSounds.Length))]);
        CharacterAnimator.SetTrigger("Shoot");
        WeaponCooldown = RifleFireRate;

        GameObject go = Instantiate(BulletPrefab, PistolObject.transform.position, PistolObject.transform.rotation);
        go.transform.LookAt(new Vector3(AttackTarget.transform.position.x, AttackTarget.transform.position.y + 1, AttackTarget.transform.position.z));
        go.GetComponent<Bullet>().SetData(gameObject, 5f);
        GameObject.Destroy(go, 4f);
    }
    private void HandleSpecialShot()
    {
        Debug.Log("Attempt Shot");
        if (WeaponCooldown > 0)
            return;

        weaponSoundSource.volume = 0.5f;
        weaponSoundSource.PlayOneShot(SpecialWeaponShootSounds[Mathf.RoundToInt(Random.Range(0, SpecialWeaponShootSounds.Length))]);
        CharacterAnimator.SetTrigger("Shoot");
        WeaponCooldown = SpecialWeaponFireRate;
    }

    public override void Attack()
    {
        if (Vector3.Distance(transform.position, DalekTarget.transform.position) > ChaseStopDistance)
        {
            AttackInProgress = false;
            AiState = State.Chase;
            DalekTarget = GameObject.Find("Player");
            ChaseTarget = DalekTarget;
        }
        else
        {
            DalekTarget = GameObject.Find("Player");
            if (DalekTarget.GetComponent<PlayerComponent>().IsAlive == false)
            {
                this.AiState = State.Idle;
                AttackInProgress = false;
            }
            else
            {
                ShootAt(DalekTarget);
            }
        }
    }

    public void ShootAt(GameObject target)
    {
        AttackTarget = target;
        AttackInProgress = true;
        Vector3 targetPostition = new Vector3(DalekTarget.transform.position.x, transform.position.y, DalekTarget.transform.position.z);
        gameObject.transform.LookAt(targetPostition);
    }

    public override void DeathBehaviour()
    {
        RifleObject.SetActive(false);
        PistolObject.SetActive(false);
        SpecialWeaponObject.SetActive(false);
    }

    public enum EquippedWeapon
    {
        Pistol,
        Rifle,
        SpecialWeapon
    }
}
