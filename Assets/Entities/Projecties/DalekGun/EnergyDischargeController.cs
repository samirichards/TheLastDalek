using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VolumetricLines;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(VolumetricLineBehavior))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Light))]

public class EnergyDischargeController : MonoBehaviour
{
    private Vector3 SpawnLocation;
    private float TravelledDistance = 0.0f;
    [SerializeField] float Range = 300f;
    [SerializeField] float ProjectileSpeed = 50f;
    public bool DestroyTarget = false; 
    private uint r;
    public uint RayType { 
        set{
            if (value > 2)
            {
                r = 2;
                return;
            }

            r = value;
            return;
        }
        get { return r; }
    }
    public AudioSource audioSource;
    private VolumetricLineBehavior lineBehavior;
    private MeshRenderer meshRenderer;
    private Light lightEmitter;
    [SerializeField] Material level1Material;
    [SerializeField] Material level2Material;
    [SerializeField] Material level3Material;
    [SerializeField] AudioClip[] ImpactSounds = new AudioClip[3];
    [SerializeField] AudioClip RichochetClip;
    [SerializeField] GameObject[] CollisionIgnore;
    [SerializeField] private GameObject CollisionExplosionPrefab;
    public Color L1_Color;
    public Color L2_Color;
    public Color L3_Color;
    public bool EnableReflection = false;
    public int MaxReflections = 1;
    private int ReflectionCount = 0;

    private float _damageStat = 100f;

    // Start is called before the first frame update
    void Start()
    {
        SpawnLocation = transform.position;
        audioSource = GetComponent<AudioSource>();
        lineBehavior = GetComponent<VolumetricLineBehavior>();
        meshRenderer = GetComponent<MeshRenderer>();
        lightEmitter = GetComponent<Light>();
    }

    public void SetData(uint _rayType, bool _destroyTarget)
    {
        RayType = _rayType;
        switch (RayType)
        {
            default:
                //GetComponent<MeshRenderer>().materials = new[] { level1Material };
                GetComponent<VolumetricLineBehavior>().LineColor = L1_Color;
                GetComponent<VolumetricLineBehavior>().TemplateMaterial = level1Material;
                GetComponent<Light>().color = L1_Color;
                _damageStat = 100f;
                break;
            case 0:
                //GetComponent<MeshRenderer>().materials = new[] { level1Material };
                GetComponent<VolumetricLineBehavior>().LineColor = L1_Color;
                GetComponent<VolumetricLineBehavior>().TemplateMaterial = level1Material;
                GetComponent<Light>().color = L1_Color;
                _damageStat = 100f;
                break;
            case 1:
                //GetComponent<MeshRenderer>().materials = new[] { level2Material };
                GetComponent<VolumetricLineBehavior>().LineColor = L2_Color;
                GetComponent<VolumetricLineBehavior>().TemplateMaterial = level2Material;
                GetComponent<Light>().color = L2_Color;
                _damageStat = 150f;
                break;
            case 2:
                //GetComponent<MeshRenderer>().materials = new[] { level3Material };
                GetComponent<VolumetricLineBehavior>().LineColor = L3_Color;
                GetComponent<VolumetricLineBehavior>().TemplateMaterial = level3Material;
                GetComponent<Light>().color = L3_Color;
                _damageStat = 200f;
                break;
        }

        DestroyTarget = _destroyTarget;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * ProjectileSpeed;
        CalculateDistance();
    }

    private void FixedUpdate()
    {
        if (TravelledDistance > 1)
        {
            GetComponent<CapsuleCollider>().enabled = true;
        }
        if (TravelledDistance > Range)
        {
            Destroy(gameObject);
        }
    }

    public void SetFireSound(AudioClip soundEffect)
    {
        //audioSource.clip = soundEffect;
        //audioSource.Play();
    }

    private void CalculateDistance()
    {
        TravelledDistance = Vector3.Distance(SpawnLocation, gameObject.transform.position);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Energy discharge hit: " + other.gameObject.name + " | " + other.gameObject.tag);
        if (!(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerShield")))
        {
            if (EnableReflection == false || ReflectionCount >= MaxReflections)
            {

                if (other.gameObject.GetComponent<BaseAI>() != null || other.gameObject.GetComponentInParent<BaseAI>() != null)
                {
                    AudioSource.PlayClipAtPoint(ImpactSounds[RayType], transform.position);

                    var dissolveColour = GetComponent<Light>().color;
                    other.gameObject.GetComponent<BaseAI>().Damage(new DamageInfo(_damageStat, gameObject, DamageType.DeathRay, DestroyTarget, dissolveColour * 150));
                    Destroy(gameObject);
                    return;
                }

                if (other.gameObject.GetComponent<DamageableComponent>() != null || other.gameObject.GetComponentInParent<DamageableComponent>() != null)
                {
                    AudioSource.PlayClipAtPoint(RichochetClip, transform.position);
                    other.gameObject.GetComponent<DamageableComponent>().Damage(new DamageInfo(_damageStat, gameObject, DamageType.DeathRay));
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    AudioSource.PlayClipAtPoint(RichochetClip, transform.position);
                    var collision = Instantiate(CollisionExplosionPrefab, transform.position, Quaternion.FromToRotation(transform.position, Vector3.Reflect(transform.position, other.contacts[0].normal)));
                    collision.GetComponent<EnergyDischargeCollisionController>().SetLightType(RayType);
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                Destroy(gameObject);
                return;
                //GetComponent<Rigidbody>().velocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, other.contacts[0].normal);
                //ReflectionCount++;
            }
        }
        Destroy(gameObject);
        return;
    }
}
