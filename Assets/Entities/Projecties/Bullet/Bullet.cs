using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 SpawnLocation;
    private float TravelledDistance = 0.0f;
    [SerializeField] float Range = 100f;
    [SerializeField] float ProjectileSpeed = 50f;
    public AudioSource audioSource;
    public AudioClip RichochetClip;
    public GameObject BulletOriginalSource;
    public float defaultAttackStrength = 10f;
    public float friendlyFireModifier = 0.666f;


    // Start is called before the first frame update
    void Start()
    {
        SpawnLocation = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * ProjectileSpeed;
        CalculateDistance();
        if (TravelledDistance > 1)
        {
            GetComponent<CapsuleCollider>().enabled = true;
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

    public void SetData(GameObject source)
    {
        BulletOriginalSource = source;
    }

    public void SetData(GameObject source, float attackStrengh)
    {
        BulletOriginalSource = source;
        defaultAttackStrength = attackStrengh;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Bullet hit: " + other.gameObject.tag);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var _damageInfo = new DamageInfo(defaultAttackStrength, BulletOriginalSource, DamageType.Bullet);
            _damageInfo.ImpactLocation = this.transform;
            other.GetComponent<PlayerComponent>().Damage(_damageInfo);
            Destroy(gameObject);
        }
        if (other.CompareTag("NPC"))
        {
            //If the bullet hits another NPC, it should do a bit less damage
            var _damageInfo = new DamageInfo(defaultAttackStrength * friendlyFireModifier, BulletOriginalSource, DamageType.Bullet);
            _damageInfo.ImpactLocation = this.transform;
            other.GetComponent<BaseAI>().Damage(_damageInfo);
            Destroy(gameObject);
        }
        if (other.tag == "Terrain")
        {
            AudioSource.PlayClipAtPoint(RichochetClip, transform.position);
            Destroy(gameObject);
        }
        AudioSource.PlayClipAtPoint(RichochetClip, transform.position);
        Destroy(gameObject);
    }
}
