using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyPelletController : MonoBehaviour
{
    private Vector3 SpawnLocation;
    private float TravelledDistance = 0.0f;
    [SerializeField] float Range = 100f;
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
        AudioSource.PlayClipAtPoint(HitSounds[Random.Range(0, HitSounds.Length)], transform.position);

    }

    private void FixedUpdate()
    {
        if (TravelledDistance > 0.5 && ProjectileSpeed > 0)
        {
            GetComponent<SphereCollider>().enabled = true;
        }
        else if(ProjectileSpeed == 0f)
        {
            GetComponent<SphereCollider>().enabled = false;
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
        Debug.Log("Energy pellet hit: " + other.gameObject.tag);
        if (other.gameObject.GetComponent<DamageableComponent>())
        {
            other.gameObject.GetComponent<DamageableComponent>().Damage(stats);
            StartCoroutine(DeleteThis(5f));
            return;

        }
        if (other.gameObject.GetComponent<BaseAI>())
        {
            other.gameObject.GetComponent<BaseAI>().Damage(stats);
            StartCoroutine(DeleteThis(5f));
            return;
        }
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerComponent>().Damage(stats);
            StartCoroutine(DeleteThis(5f));
            return;
        }
        if (other.tag == "Hole")
        {
            return;
        }
        if (other.tag == "Terrain")
        {
            StartCoroutine(DeleteThis(5f));
            return;
        }
        StartCoroutine(DeleteThis(5f));
        return;
    }

    IEnumerator DeleteThis(float time)
    {
        ProjectileSpeed = 0f;
        this.GetComponentInChildren<MeshRenderer>().enabled = false;
        this.GetComponentInChildren<VisualEffect>().Stop();
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
