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

    [Header("General")]
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
    public delegate void NPCDeathHandler(BaseAI npc);
    public static event NPCDeathHandler OnNPCDeath;
    private int DeathRayHitCounter = 0;
    List<Material> originalMaterials;
    List<Coroutine> deathCoroutines = new List<Coroutine>();
    private bool StartedDissolveEffect = false;
    [SerializeField] private GameObject Head;


    [Header("Extermination Effects")]
    [SerializeField] protected GameObject SkeletonObject;
    [SerializeField] protected GameObject MainBody;
    [SerializeField] protected Material SkinExterminationMaterial;
    [SerializeField] public float SkeletonRevealTime = 0.666f;


    [Header("AI:General")]
    [SerializeField] protected State AiState;
    [SerializeField] protected EmotionState Emotion;
    [SerializeField] protected NPCType npcType;

    //Idle Variables
    [Header("AI:Idle")]
    [SerializeField] protected float MaxIdleTime = 10.0f;
    protected float IdleTime = 0.0f;

    //Wander Variables
    [Header("AI:Wander")]
    [SerializeField] protected float MaxWanderRadius = 30.0f;
    protected Vector3 WanderDestination;
    protected bool WanderLocationSet = false;
    [SerializeField] protected float MaxWanderCooldown = 5.0f;
    protected float WanderCooldown = 0.0f;

    //Flee Variables
    [Header("AI:Flee")]
    [SerializeField] protected float MaxFleeRadius = 50.0f;
    protected Vector3 FleeDestination;
    protected bool FleeLocationSet = false;
    [SerializeField] protected float MaxFleeCooldown = 2.0f;
    protected float FleeCooldown = 0.0f;

    //Attack/chase Variables
    [Header("AI:Attack/chase")]
    [SerializeField] protected float AttackDistance = 12.0f;
    [SerializeField] protected float ChaseStopDistance = 12.0f;
    [SerializeField] protected GameObject ChaseTarget;
    [SerializeField] protected float ChaseLoseDistance = 65.0f;
    [SerializeField] protected float ChaseLoseTime = 6.0f;
    protected float DropChaseTimer = 0.0f;

    //Patrol Variables
    [Header("AI:Patrol")]
    [SerializeField] protected Transform[] patrolPoints;
    protected int CurrentPatrolIndex = 0;
    [SerializeField] protected bool patrolIsCyclical;
    [SerializeField] protected bool walkPatrolPath;
    [SerializeField] protected bool returnToPathWhenPossible;

    [SerializeField] protected float HeadlessChickenFactor = 1f;

    [Header("Sounds")]
    [SerializeField] protected AudioClip[] DamageVOs;
    [SerializeField] protected AudioClip[] HitSounds;
    [SerializeField] protected AudioClip[] DeathSounds;
    [SerializeField] protected AudioSource SoundSource;

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
        _gameManagerComponent = GameManager.Instance;
        CharacterAnimator = MainBody.GetComponent<Animator>();
        PlayerSpawner.OnDalekSpawned += OnDalekSpawned;
        originalMaterials = new List<Material>();
        foreach (SkinnedMeshRenderer meshRenderer in MainBody.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            originalMaterials.Add(meshRenderer.material);
        }
    }

    private void OnDestroy()
    {
        PlayerSpawner.OnDalekSpawned -= OnDalekSpawned;
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
        CharacterAnimator = MainBody.GetComponent<Animator>();
        CharacterAnimator.SetInteger("AnimationState", 0);
        Health = MaxHealth;
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        agent.updatePosition = true;
        agent.updateRotation = true;
        SetDefaults();
        StartCoroutine("FSM");
    }

    protected virtual void OnDalekSpawned(object sender, PlayerSpawnedArgs e)
    {
        DalekTarget = GameObject.FindGameObjectWithTag("Player");
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
        if (DalekTarget == null)
        {
            DalekTarget = GameObject.FindGameObjectWithTag("Player");
        }
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
            if (!GameManager.IsGamePaused && DalekTarget != null)
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
            if (DalekInLOS && DalekTarget.GetComponent<PlayerComponent>().IsAlive)
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
        agent.isStopped = true;
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
                    ChaseTarget = DalekTarget;
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
                    ChaseTarget = DalekTarget;
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

    public void PlungerSlowKill(DamageInfo source)
    {
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        agent.enabled = false;
        controller.enabled = false;
        controller.Move(Vector3.zero);
        MaxSpeed = 0.0f;


        var plungerHead = source.DamageSource.GetComponentInChildren<PropController>().getPlungerHead;
        var plungerForward = plungerHead.transform.forward;
        var offsetPosition = plungerHead.transform.position + plungerForward * -2f; 

        this.transform.position = offsetPosition;

        Ragdoll();

        CharacterAnimator.SetInteger("AnimationState", -1);
        AiState = State.Dead;
        IsAlive = false;
        GameManager.IncrementExterminations();
        OnNPCDeath?.Invoke(this);
        DeathBehaviour();
        CharacterAnimator.SetBool("IsAlive", false);
        StartCoroutine(ConnectHeadWithDalekPlunger(3.25f, plungerHead));
        StartCoroutine(DisableCollisionAfterTime(0.01f));
    }

    IEnumerator ConnectHeadWithDalekPlunger(float duration, GameObject plungerHead)
    {
        var currentTime = 0f;

        // Ensure the head's Rigidbody is kinematic
        var headRigidbody = Head.GetComponent<Rigidbody>();
        if (headRigidbody != null)
        {
            headRigidbody.isKinematic = true;
        }

        while (currentTime < duration)
        {
            Head.transform.position = plungerHead.transform.position + (plungerHead.transform.forward * -0.25f);

            Vector3 directionToPlunger = (plungerHead.transform.position - Head.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlunger);
            Head.transform.rotation = targetRotation;

            currentTime += Time.deltaTime;
            yield return null;
        }

        // Optionally reset the Rigidbody state after the coroutine
        if (headRigidbody != null)
        {
            headRigidbody.isKinematic = false;
        }
        deathCoroutines.Add(StartCoroutine(DeleteNPC(10)));
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
        DeathRayHitCounter++;
        if (DeathRayHitCounter > 1 && IsAlive == false && StartedDissolveEffect == false)
        {
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Collider>().enabled = false;
            StartedDissolveEffect = true;
            foreach (var item in deathCoroutines)
            {
                StopCoroutine(item);
            }
            var mats = originalMaterials;
            foreach (SkinnedMeshRenderer meshRenderer in MainBody.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                meshRenderer.material = mats[0];
                mats.RemoveAt(0);
            }
            mats.Clear();
            SkeletonObject.SetActive(false);
            Ragdoll();
            CharacterAnimator.SetBool("IsAlive", false);
            deathCoroutines.Add(StartCoroutine(Dissolve(0.33f, _damageInfo)));
            StartCoroutine(DeleteNPC(0.5f));
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

    void Freeze()
    {
        Rigidbody[] npcRigid = MainBody.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigid in npcRigid)
        {
            rigid.isKinematic = true;
        }
        CharacterAnimator.enabled = false;
    }

    void Ragdoll()
    {
        //Shifts the model up slightly, as well as quickly making the model kinematic then normal, which resets any momentum the rigidbodies have
        Vector3 newTransform = MainBody.transform.position;
        //newTransform.y += 0.1f;
        MainBody.transform.position = newTransform; 
        Rigidbody[] npcRigid = MainBody.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigid in npcRigid)
        {
            rigid.isKinematic = true;
        }
        CharacterAnimator.enabled = false;
        foreach (Rigidbody rigid in npcRigid)
        {
            rigid.isKinematic = false;
        }
    }

    public virtual void DeathBehaviour()
    {

    }

    void Die(DamageInfo _damageInfo)
    {
        GameManager.IncrementExterminations();
        SoundSource.PlayOneShot(DeathSounds[Mathf.RoundToInt(Random.Range(0, DeathSounds.Length))]);
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        agent.enabled = false;
        controller.enabled = false;
        controller.Move(Vector3.zero);
        MaxSpeed = 0.0f;
        StartCoroutine(DisableCollisionAfterTime(SkeletonRevealTime));
        AiState = State.Dead;
        IsAlive = false;
        //CharacterAnimator.enabled = false;
        OnNPCDeath?.Invoke(this);
        DeathBehaviour();
        if (_damageInfo.DamageType == DamageType.DeathRay)
        {
            try
            {
                if (!_damageInfo.DestroyTarget)
                {
                    //var energyDischarge = _damageInfo.DamageSource.GetComponent<EnergyDischargeController>();
                    //if (energyDischarge != null)
                    //{
                    //    switch (energyDischarge.RayType)
                    //    {
                    //        case 0:
                    //            SkinExterminationMaterial.SetColor("_EmissionColour", energyDischarge.L1_Color * 30);
                    //            SkinExterminationMaterial.SetColor("_SkinColour", energyDischarge.L1_Color * 30);
                    //            break;
                    //        case 1:
                    //            SkinExterminationMaterial.SetColor("_EmissionColour", energyDischarge.L2_Color * 30);
                    //            SkinExterminationMaterial.SetColor("_SkinColour", energyDischarge.L2_Color * 30);
                    //            break;
                    //        case 2:
                    //            SkinExterminationMaterial.SetColor("_EmissionColour", energyDischarge.L3_Color * 30);
                    //            SkinExterminationMaterial.SetColor("_SkinColour", energyDischarge.L3_Color * 30);
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}

                    SkeletonObject.SetActive(true);
                    CharacterAnimator.SetTrigger("Exterminate");
                    SkeletonObject.GetComponent<Animator>().SetTrigger("Exterminate");
                    deathCoroutines.Add(StartCoroutine(RagdollTimer(5)));
                    deathCoroutines.Add(StartCoroutine(SkeletonReveal(SkeletonRevealTime)));
                    CharacterAnimator.SetBool("IsAlive", false);
                    deathCoroutines.Add(StartCoroutine(DeleteNPC(10)));
                }
                else
                {
                    foreach (var item in MainBody.GetComponentsInChildren<Rigidbody>())
                    {
                        item.useGravity = false;
                    }
                    Ragdoll();
                    CharacterAnimator.speed = 0;
                    CharacterAnimator.SetBool("IsAlive", false);
                    deathCoroutines.Add(StartCoroutine(Dissolve(1.5f, _damageInfo)));
                    deathCoroutines.Add(StartCoroutine(DeleteNPC(4f)));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return;
        }
        if (_damageInfo.DamageType == DamageType.Electric)
        {
            try
            {
                SkeletonObject.SetActive(true);
                CharacterAnimator.SetTrigger("Electrocute");
                SkeletonObject.GetComponent<Animator>().SetTrigger("Electrocute");
                deathCoroutines.Add(StartCoroutine(RagdollTimer(5)));
                deathCoroutines.Add(StartCoroutine(SkeletonReveal(2f)));
                CharacterAnimator.SetBool("IsAlive", false);
                deathCoroutines.Add(StartCoroutine(DeleteNPC(10)));
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return;
        }
        CharacterAnimator.SetInteger("AnimationState", -1);
        Ragdoll();
        //CharacterAnimator.Play(Animator.StringToHash("Exterminating"), -1, 0.125f);
    }

    IEnumerator DisableCollisionAfterTime(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    IEnumerator SkeletonReveal(float SkeletonRevealTime)
    {
        //This approach is done since some models can have several meshes with several materials, this prevents only part of the model showing the skeleton
        var mats = originalMaterials;
        foreach (SkinnedMeshRenderer meshRenderer in MainBody.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            meshRenderer.material = SkinExterminationMaterial;
        }

        // Wait for a brief duration
        yield return new WaitForSeconds(SkeletonRevealTime);

        // Switch back to the original materials
        foreach (SkinnedMeshRenderer meshRenderer in MainBody.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            meshRenderer.material = mats[0];
            mats.RemoveAt(0);
        }
        mats.Clear();

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

    IEnumerator RagdollTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Ragdoll(); 
    }


    IEnumerator DeleteNPC(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    IEnumerator Dissolve(float time, DamageInfo damageInfo)
    {
        //Fairly heavy operation, but it only happens on death so idc
        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        float currentTime = 0;


        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            foreach (var material in skinnedMeshRenderer.materials)
            {
                material.SetColor("_DissolveColour", damageInfo.DissolveColour);
            }
        }

        while (skinnedMeshRenderers[0].materials[0].GetFloat("_DissolveAmount") < 1 && currentTime < time)
        {
            foreach (var skinnedMesh in skinnedMeshRenderers)
            {
                if (skinnedMesh.materials.Length > 0)
                {
                    foreach (var material in skinnedMesh.materials)
                    {
                        
                        material.SetFloat("_DissolveAmount", currentTime / time);
                    }
                    
                }
            }
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        foreach (var skinnedMesh in skinnedMeshRenderers)
        {
            foreach (var material in skinnedMesh.materials)
            {
                material.SetFloat("_DissolveAmount", 1);
            }
        }
    }
}
