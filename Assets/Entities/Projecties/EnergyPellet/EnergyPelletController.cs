using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPelletController : MonoBehaviour
{
    private Vector3 SpawnLocation;
    private float TravelledDistance = 0.0f;
    [SerializeField] float Range = 300f;
    [SerializeField] float ProjectileSpeed = 50f;
    [SerializeField] GameObject[] CollisionIgnore;
    [SerializeField] private AudioClip[] HitSounds;
    [SerializeField]private DamageInfo stats;

    public void SetData(DamageInfo info, float projectileSpeed)
    {
        stats = info;
        ProjectileSpeed = projectileSpeed;
    }

    public void SetData(DamageInfo info)
    {
        stats = info;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnLocation = transform.position;
    }

    private void FixedUpdate()
    {
        if (TravelledDistance > 0.5)
        {
            GetComponent<SphereCollider>().enabled = true;
        }
        if (TravelledDistance > Range)
        {
            Destroy(gameObject);
        }
    }

    private void CalculateDistance()
    {
        TravelledDistance = Vector3.Distance(SpawnLocation, gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * ProjectileSpeed;
        CalculateDistance();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Energy pellet hit: " + other.gameObject.tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<DamageableComponent>())
        {
            AudioSource.PlayClipAtPoint(HitSounds[Random.Range(0, HitSounds.Length)], transform.position);
            other.gameObject.GetComponent<DamageableComponent>().Damage(stats);
        }
        if (other.gameObject.GetComponent<BaseAI>())
        {
            AudioSource.PlayClipAtPoint(HitSounds[Random.Range(0, HitSounds.Length)], transform.position);
            other.gameObject.GetComponent<BaseAI>().Damage(stats);
        }
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerComponent>().Damage(stats);
            Destroy(gameObject);
        }
        if (other.tag == "Terrain")
        {
            AudioSource.PlayClipAtPoint(HitSounds[Random.Range(0, HitSounds.Length)], transform.position);
            Destroy(gameObject);
        }
        AudioSource.PlayClipAtPoint(HitSounds[Random.Range(0, HitSounds.Length)], transform.position);
        Destroy(gameObject);
    }
}
