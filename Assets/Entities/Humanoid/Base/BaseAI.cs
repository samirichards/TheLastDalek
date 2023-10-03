using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class BaseAI : MonoBehaviour
{
    private GameManager _gameManagerComponent;
    protected Animator CharacterAnimator;

    public float MaxHealth = 100f;

    public float Health;

    [SerializeField] protected float MaxSpeed = 2.6f;
    [SerializeField] protected float WalkSpeedPercentage = 0.5f;
    [SerializeField] protected float FearfulWalkSpeedPercentage = 0.25f;
    [SerializeField] protected float speed = 0.0f;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected CharacterController controller;
    [SerializeField] public bool IsAlive = true;
    [SerializeField] protected float SightRange = 30.0f;
    [SerializeField] protected GameObject DalekTarget;
    [SerializeField] protected LayerMask TargetMask;
    [SerializeField] protected bool DalekInLOS = false;
    [SerializeField] protected bool isOnGround = true;
    [SerializeField] protected AudioClip[] DamageVOs;
    [SerializeField] protected AudioClip[] HitSounds;
    [SerializeField] protected AudioClip[] DeathSounds;
    [SerializeField] protected AudioSource SoundSource;
    [SerializeField] protected State AiState;
    [SerializeField] protected EmotionState Emotion;
    [SerializeField] protected NPCType npcType;

    [SerializeField] protected GameObject SkeletonObject;
    [SerializeField] protected GameObject MainBody;
    [SerializeField] protected Material SkinExterminationMaterial;
    private Material[] originalMaterial;


    //Idle Variables
    [SerializeField] protected float MaxIdleTime = 10.0f;
    protected float IdleTime = 0.0f;

    //Wander Variables
    [SerializeField] protected float MaxWanderRadius = 30.0f;
    protected Vector3 WanderDestination;
    protected bool WanderLocationSet = false;
    [SerializeField] protected float MaxWanderCooldown = 5.0f;
    protected float WanderCooldown = 0.0f;

    //Flee Variables
    [SerializeField] protected float MaxFleeRadius = 50.0f;
    protected Vector3 FleeDestination;
    protected bool FleeLocationSet = false;
    [SerializeField] protected float MaxFleeCooldown = 2.0f;
    protected float FleeCooldown = 0.0f;

    //Attack/chase Variables
    [SerializeField] protected float AttackDistance = 12.0f;
    [SerializeField] protected float ChaseStopDistance = 12.0f;
    [SerializeField] protected GameObject ChaseTarget;
    [SerializeField] protected float ChaseLoseDistance = 65.0f;
    [SerializeField] protected float ChaseLoseTime = 6.0f;
    protected float DropChaseTimer = 0.0f;

    //Patrol Variables
    [SerializeField] protected Transform[] patrolPoints;
    protected int CurrentPatrolIndex = 0;
    [SerializeField] protected bool patrolIsCyclical;
    [SerializeField] protected bool walkPatrolPath;
    [SerializeField] protected bool returnToPathWhenPossible;

    [SerializeField] protected float HeadlessChickenFactor = 1f;

    public delegate void NPCDeathHandler(BaseAI npc);
    public static event NPCDeathHandler OnNPCDeath;

    public enum State
    {
        Idle,
        Wander,
        Flee,
        Patrol,
        Chase,
        Attack,
        Investigate,
        Dead,
        Stunned,
        HeadlessChicken
    }

    public enum EmotionState
    {
        Normal,
        Scared,
        Angry,
        Curious
    }

    public enum NPCType
    {
        Passive,
        Hostile,
        Special
    }

    void Awake()
    {
        _gameManagerComponent = DalekTarget.GetComponent<GameManager>();
        CharacterAnimator = GetComponentInChildren<Animator>();
        try
        {
            SkeletonCollisionFix();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// Override this to set AiState, Default Emotion, and npcType
    /// </summary>
    public virtual void SetDefaults()
    {
        Emotion = EmotionState.Normal;
        npcType = NPCType.Passive;
    }

    void Start()
    {
        CharacterAnimator.SetInteger("AnimationState", 0);
        CharacterAnimator = GetComponentInChildren<Animator>();
        Health = MaxHealth;
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        agent.updatePosition = true;
        agent.updateRotation = true;
        DalekTarget = Player.Instance.gameObject;
        SetDefaults();
        StartCoroutine("FSM");
        try
        {
            SkeletonCollisionFix();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return transform.position; // Return current position if no valid position is found
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Vector3.Distance(transform.position, DalekTarget.transform.position) <= SightRange && IsAlive)
        {
            Vector3 origin = transform.position;
            origin.y += 0.5f;


            var target = new Vector3(DalekTarget.transform.position.x, DalekTarget.transform.position.y +1,
                DalekTarget.transform.position.z);

            var direction = (target - transform.position).normalized;


            if (Physics.Raycast(origin, direction, out hit, SightRange, TargetMask.value))
            {
                
                if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "PlayerShield")
                {
                    DalekInLOS = true;
                }
                else
                {
                    DalekInLOS = false;
                }
            }
            else
            {
                Debug.Log("Not that ig");
            }
        }
        else
        {
            DalekInLOS = false;
        }
        CharacterAnimator.SetBool("IsFearful", Emotion == EmotionState.Scared);
    }

    IEnumerator FSM()
    {
        while (IsAlive)
        {
            if (!GameManager.IsGamePaused)
            {
                switch (AiState)
                {
                    case State.Idle:
                        Idle();
                        break;
                    case State.Wander:
                        Wander();
                        break;
                    case State.Flee: 
                        Flee();
                        break;
                    case State.Patrol: 
                        Patrol();
                        break;
                    case State.Chase: 
                        Chase();
                        break;
                    case State.Investigate:
                        Investigate();
                        break;
                    case State.Attack: 
                        Attack(); 
                        break;
                    case State.Dead: 
                        Dead(); 
                        break;
                    case State.Stunned:
                        Stunned();
                        break;
                    case State.HeadlessChicken:
                        HeadlessChicken();
                        break;
                    default:
                        Idle();
                        break;
                }
                yield return null;
            }
            yield return null;
        }
        yield return null;
    }

    public virtual void Idle()
    {
        agent.speed = 0;
        agent.SetDestination(gameObject.transform.position);
        CharacterAnimator.SetInteger("AnimationState", 0);
        if (Random.Range(0f, 1f) < 0.03f)
        {
            AiState = State.Wander;
        }
        else
        {
            if (DalekInLOS)
            {
                switch (npcType)
                {
                    case NPCType.Passive:
                        AiState = State.Flee;
                        break;
                    case NPCType.Hostile:
                        AiState = State.Chase;
                        break;
                    default:
                        AiState = State.Flee; 
                        break;
                }
            }
        }

    }

    public virtual void Wander()
    {
        agent.speed = MaxSpeed * WalkSpeedPercentage;
        if (WanderLocationSet)
        {
            if (Vector3.Distance(this.transform.position, WanderDestination) >= 1.0f)
            {
                agent.SetDestination(WanderDestination);
                controller.Move(agent.desiredVelocity);
            }
            else
            {
                WanderCooldown = MaxWanderCooldown;
                WanderLocationSet = false;
            }
        }
        else
        {
            if (WanderCooldown > 0)
            {
                WanderCooldown -= Time.deltaTime;
            }
            else
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    AiState = State.Idle;
                }
                else
                {
                    WanderDestination = RandomNavMeshLocation(MaxWanderRadius);
                    WanderLocationSet = true;
                }
            }
        }
    }

    public virtual void Stunned()
    {
        WanderCooldown = 0;
        FleeCooldown = 0;
        WanderLocationSet = false;
        FleeLocationSet = false;
        agent.Stop();
        agent.SetDestination(transform.position);
        if (npcType == NPCType.Passive)
        {
            AiState = State.Flee;
        }

        if (npcType == NPCType.Hostile)
        {
            if (Health / MaxHealth <= 0.3333333)
            {
                AiState = State.Flee;
            }
            else
            {
                AiState = State.Attack;
            }
        }

        agent.isStopped = false;

    }

    public virtual void Flee()
    {
        agent.speed = MaxSpeed;
        if (FleeLocationSet)
        {
            if (Vector3.Distance(this.transform.position, FleeDestination) >= 1.0f)
            {
                agent.SetDestination(FleeDestination);
            }
            else
            {
                FleeCooldown = Random.Range(0.5f, MaxFleeCooldown);
                FleeLocationSet = false;
            }
        }
        else
        {
            if (FleeCooldown > 0)
            {
                FleeCooldown -= Time.deltaTime;
            }
            else
            {
                if (DalekTarget)
                {
                    FleeDestination = GetFarthestPointFromDalek(MaxFleeRadius);
                }
                else
                {
                    FleeDestination = GetFarthestPointFromDalek(MaxFleeRadius);
                }
                FleeLocationSet = true;
            }
        }
    }

    public virtual void Patrol()
    {
        if (patrolPoints.Length == 0)
            AiState = State.Wander;

        agent.autoBraking = false;
        agent.destination = patrolPoints[CurrentPatrolIndex].position;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (patrolIsCyclical)
            {
                if (CurrentPatrolIndex == patrolPoints.Length)
                {
                    Array.Reverse(patrolPoints);
                }
            }
            CurrentPatrolIndex = (CurrentPatrolIndex + 1) % patrolPoints.Length;
        }

        if (DalekInLOS)
        {
            switch (npcType)
            {
                case NPCType.Passive:
                    AiState = State.Flee;
                    break;
                case NPCType.Hostile:
                    ChaseTarget = GameObject.Find("Player");
                    AiState = State.Chase;
                    break;
                default:
                    AiState = State.Flee;
                    break;
            }
        }

        controller.Move(agent.desiredVelocity);
        agent.isStopped = false;

    }

    public virtual void Chase()
    {
        DalekTarget = GameObject.Find("Player");
        ChaseTarget = DalekTarget;
        agent.speed = MaxSpeed;
        if (ChaseTarget != null)
        {
            if (Vector3.Distance(this.transform.position, ChaseTarget.transform.position) >= ChaseStopDistance)
            {
                agent.SetDestination(ChaseTarget.transform.position);
                controller.Move(agent.desiredVelocity);
                DropChaseTimer = 0f;
            }
            else
            {
                if (Vector3.Distance(this.transform.position, ChaseTarget.transform.position) >= ChaseStopDistance)
                {
                    DropChaseTimer += Time.deltaTime;
                    if (DropChaseTimer >=ChaseLoseTime)
                    {
                        if (returnToPathWhenPossible)
                        {
                            AiState = State.Patrol;
                        }
                        else
                        {
                            AiState = State.Idle;
                        }
                    }
                }
                else
                {
                    DropChaseTimer = 0f;
                    ChaseTarget = null;
                    agent.SetDestination(this.transform.position);
                    if (npcType == NPCType.Hostile)
                    {
                        AiState = State.Attack;
                    }
                }
            }
        }
    }

    public virtual void Attack()
    {

    }

    public virtual void Investigate()
    {

    }

    public virtual void HeadlessChicken()
    {

    }

    Vector3 GetFarthestPointFromDalek(float maxRange)
    {
        Vector3 farthestPoint = Vector3.zero;
        float farthestDistance = 0f;
        Vector3 agentPosition = transform.position;

        int numDirections = 32;
        for (int i = 0; i < numDirections; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            Vector3 fleePosition = agentPosition + randomDirection * maxRange;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(fleePosition, out navHit, maxRange, NavMesh.AllAreas))
            {
                float distanceToDalek = Vector3.Distance(navHit.position, DalekTarget.transform.position);

                if (distanceToDalek > farthestDistance)
                {
                    farthestPoint = navHit.position;
                    farthestDistance = distanceToDalek;
                }
            }
        }

        return farthestPoint;
    }

    Vector3 GetRandomPointAroundPosition(float maxRange, Vector3 location)
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(0f, maxRange);
        Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

        return location + direction * distance;
    }

    public virtual void Dead()
    {
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        agent.enabled = false;
        controller.enabled = false;
        controller.Move(Vector3.zero);
        MaxSpeed = 0.0f;
    }

    public void Damage(DamageInfo _damageInfo)
    {
        Health -= _damageInfo.DamageValue;
        if (_damageInfo.DamageType != DamageType.DeathRay)
        {
            SoundSource.PlayOneShot(HitSounds[Mathf.RoundToInt(Random.Range(0, HitSounds.Length))]);
        }
        if (_damageInfo.DamageValue / MaxHealth >= 0.45)
        {
            AiState = State.Stunned;
            CharacterAnimator.SetTrigger("Damage_Large");
        }
        else if (_damageInfo.DamageValue / MaxHealth >= 0.3333333333333)
        {
            AiState = State.Stunned;
            CharacterAnimator.SetTrigger("Damage_Small");
        }

        if (Health / MaxHealth <= 0.25)
        {
            Emotion = EmotionState.Scared;
            AiState = State.Flee;
        }
        if (Health <= 0f && IsAlive)
        {
            Health = 0f;
            Die(_damageInfo);
        }
        else
        {
            SoundSource.PlayOneShot(DamageVOs[Mathf.RoundToInt(Random.Range(0, HitSounds.Length))]);
            //Don't play a hit vo if they will be dead
        }
    }

    public virtual void CustomUpdateBehaviour()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CharacterAnimator.SetFloat("MovementMagnitude", agent.velocity.magnitude / MaxSpeed);
        isOnGround = controller.isGrounded;
        CustomUpdateBehaviour();
    }

    void SkeletonCollisionFix()
    {
        Collider npcCollider = MainBody.GetComponent<Collider>();
        Collider[] skeletonColliders = SkeletonObject.GetComponentsInChildren<Collider>();

        foreach (Collider skeletonCollider in skeletonColliders)
        {
            Physics.IgnoreCollision(skeletonCollider, npcCollider);
        }
    }

    public virtual void DeathBehaviour()
    {

    }

    void Die(DamageInfo _damageInfo)
    {
        GameManager.IncrementExterminations();
        CharacterAnimator.SetBool("IsAlive", false);
        SoundSource.PlayOneShot(DeathSounds[Mathf.RoundToInt(Random.Range(0, DeathSounds.Length))]);
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        agent.enabled = false;
        controller.enabled = false;
        controller.Move(Vector3.zero);
        MaxSpeed = 0.0f;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Collider>().enabled = false;
        AiState = State.Dead;
        IsAlive = false;
        CharacterAnimator.enabled = false;
        OnNPCDeath?.Invoke(this);
        DeathBehaviour();
        if (_damageInfo.DamageType == DamageType.DeathRay)
        {
            originalMaterial = MainBody.GetComponentInChildren<SkinnedMeshRenderer>().materials;
            MainBody.GetComponentInChildren<SkinnedMeshRenderer>().materials = new Material[1]{ SkinExterminationMaterial };
            try
            {
                SkeletonObject.SetActive(true);
                CharacterAnimator.SetTrigger("Exterminate");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            StartCoroutine(SwitchBackMaterials());
        }
        CharacterAnimator.SetInteger("AnimationState", -1);
        //CharacterAnimator.Play(Animator.StringToHash("Exterminating"), -1, 0.125f);
    }

    IEnumerator SwitchBackMaterials()
    {
        // Wait for a brief duration
        yield return new WaitForSeconds(0.333f); // Adjust the duration as needed

        // Switch back to the original materials
        MainBody.GetComponentInChildren<SkinnedMeshRenderer>().materials = originalMaterial;

        try
        {
            // Disable the skeleton object
            SkeletonObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
