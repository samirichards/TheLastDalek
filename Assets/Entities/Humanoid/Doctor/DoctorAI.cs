using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DoctorAI : BaseAI
{
    [SerializeField] protected GameObject SonicObject;
    [SerializeField] protected EquippedWeapon equippedWeapon;
    [SerializeField] protected GameObject SonicProjectile;

    [SerializeField] protected float SonicFireRate = 2f;
    [SerializeField] private AudioSource weaponSoundSource;

    [SerializeField] private AudioClip[] SonicPulseSounds;

    public float WeaponCooldown = 0.0f;
    public GameObject AttackTarget;
    public bool AttackInProgress = false;
    public override void SetDefaults()
    {
        npcType = NPCType.Hostile;
        Emotion = EmotionState.Angry;
        SonicObject.SetActive(false);
        switch (equippedWeapon)
        {
            case EquippedWeapon.Sonic:
                SonicObject.SetActive(true);
                CharacterAnimator.SetBool("WeaponType", true);
                break;
            default:
                SonicObject.SetActive(true);
                CharacterAnimator.SetBool("WeaponType", false);
                break;
        }
    }

    protected override void OnDalekSpawned(object sender, PlayerSpawnedArgs e)
    {
        DalekTarget = GameObject.FindGameObjectWithTag("Player");
        AttackTarget = GameObject.FindGameObjectWithTag("Player");
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
                case EquippedWeapon.Sonic:
                    HandleSonic();
                    break;
                default:
                    HandleSonic();
                    break;
            }
        }

    }

    private void HandleSonic()
    {
        Debug.Log("Attempt Sonic");
        if (WeaponCooldown > 0)
            return;

        weaponSoundSource.volume = 0.5f;
        weaponSoundSource.PlayOneShot(SonicPulseSounds[Mathf.RoundToInt(Random.Range(0, SonicPulseSounds.Length))]);
        CharacterAnimator.SetTrigger("Shoot");
        WeaponCooldown = SonicFireRate;

        GameObject go = Instantiate(SonicProjectile, SonicObject.transform.position, SonicObject.transform.rotation);
        go.transform.LookAt(new Vector3(AttackTarget.transform.position.x, AttackTarget.transform.position.y + 1, AttackTarget.transform.position.z));
        go.GetComponent<Bullet>().SetData(gameObject,6f);
        GameObject.Destroy(go, 3f);



    }

    public override void Attack()
    {
        if (Vector3.Distance(transform.position, DalekTarget.transform.position) > ChaseStopDistance)
        {
            AttackInProgress = false;
            AiState = State.Chase;
            ChaseTarget = DalekTarget;
        }
        else
        {
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
        SonicObject.SetActive(false);
    }

    public enum EquippedWeapon
    {
        Sonic,
    }
}
