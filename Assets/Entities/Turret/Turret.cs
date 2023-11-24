using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : DamageableComponent
{
    [SerializeField] private GameObject TurretBase;
    [SerializeField] private GameObject TurretHead;
    [SerializeField] private GameObject BarrelEnd;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private GameObject SmallExplosionPrefab;
    [SerializeField] private float AttackRange;
    [SerializeField] private float WakeRange;
    [SerializeField] private float DamageStat;
    [SerializeField] private float MaxFireRate;
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private bool BurstFire;
    [SerializeField] private bool FireInOneDirection;
    [SerializeField] private Vector3 SingleFireDirection;
    [SerializeField] private AudioClip[] DamageSounds;
    [SerializeField] private AudioClip[] FireSounds;
    [SerializeField] private float DeathCooldownMinDuration;
    public GameObject AttackTarget;
    public float DeathCooldown;
    public float Cooldown = 0.0f;
    private Animator _animator;

    public TurretState CurrentState = TurretState.Idle;

    public enum TurretState
    {
        Idle,
        Alert,
        Attacking,
        OnCooldown,
        Shutdown
    }
    // Start is called before the first frame update
    void Start()
    {
        AttackTarget = GameObject.Find("Player");
        _animator.Play("Idle");
        StartCoroutine(FSM());
        Health = MaxHealth;
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState != TurretState.Shutdown)
        {
            if (Cooldown > 0)
            {
                Cooldown -= Time.deltaTime;
            }

            if (DeathCooldown > 0)
            {
                CurrentState = TurretState.OnCooldown;
                DeathCooldown -= Time.deltaTime;
                _animator.Play("Idle");
            }

            if (CurrentState == TurretState.Alert || CurrentState == TurretState.Attacking)
            {

                // turret control:
                Vector3 targetPosTurret = new Vector3(AttackTarget.transform.position.x, TurretHead.transform.position.y, AttackTarget.transform.position.z);
                Quaternion turretRotationFinal = Quaternion.LookRotation(
                    forward: targetPosTurret - TurretHead.transform.position,
                    upwards: TurretBase.transform.up
                );
                float turretDegreesToFinal = Quaternion.Angle(TurretHead.transform.rotation, turretRotationFinal);
                TurretHead.transform.rotation = Quaternion.Lerp(
                    TurretHead.transform.rotation, turretRotationFinal,
                    (60 / turretDegreesToFinal) * Time.deltaTime
                );
            }

            if (CurrentState == TurretState.Attacking)
            {
                Shoot(DamageStat);
                _animator.Play("Alert");
            }
        }
    }

    IEnumerator FSM()
    {
        while (gameObject)
        {
            if (!GameManager.IsGamePaused && (CurrentState != TurretState.Shutdown))
            {
                if (DeathCooldown > 0)
                {
                    CurrentState = TurretState.OnCooldown;
                    _animator.SetBool("IsAlert", false);
                    _animator.Play("Idle");
                }

                if (CurrentState == TurretState.OnCooldown && DeathCooldown <= 0)
                {
                    Health = MaxHealth;
                    CurrentState = TurretState.Idle;
                    _animator.Play("Idle");
                }
                if (Vector3.Distance(transform.position, AttackTarget.transform.position) > WakeRange)
                {
                    CurrentState = TurretState.Idle;
                    _animator.SetBool("IsAlert", false);
                    _animator.Play("Idle");
                }
                if (Vector3.Distance(transform.position, AttackTarget.transform.position) < WakeRange && Vector3.Distance(transform.position, AttackTarget.transform.position) > AttackRange && (CurrentState != TurretState.OnCooldown || CurrentState != TurretState.Shutdown))
                {
                    CurrentState = TurretState.Alert;
                    _animator.SetBool("IsAlert", true);
                    _animator.Play("Alert");
                }
                if (Vector3.Distance(transform.position, AttackTarget.transform.position) < AttackRange && (CurrentState != TurretState.OnCooldown || CurrentState != TurretState.Shutdown))
                {
                    CurrentState = TurretState.Attacking;
                    _animator.SetBool("IsAlert", true);
                    _animator.Play("Alert");
                }
            }
            yield return null;
        }
        yield return null;
    }

    void FixedUpdate()
    {
        
    }

    void Shoot(float damage)
    {
        if (Cooldown <= 0)
        {
            GameObject go = Instantiate(Projectile, BarrelEnd.transform.position, BarrelEnd.transform.rotation);
            go.GetComponent<EnergyPelletController>().SetData(new DamageInfo(DamageStat, this.gameObject, DamageType.EnergyDischarge), ProjectileSpeed);
            Cooldown = 1 / MaxFireRate;
            Destroy(go, 3f);
        }
    }

    public void Shutdown()
    {
        CurrentState = TurretState.Shutdown;
    }

    protected override void OnDamage(DamageInfo damageInfo)
    {
        base.OnDamage(damageInfo);
        GetComponent<AudioSource>().PlayOneShot(DamageSounds[Random.Range(0, DamageSounds.Length)]);
    }

    protected override void OnBreak(DamageInfo damageInfo)
    {
        DeathCooldown = DeathCooldownMinDuration;
        CurrentState = TurretState.OnCooldown;
        Instantiate(SmallExplosionPrefab, transform.position, transform.rotation);
    }
}
