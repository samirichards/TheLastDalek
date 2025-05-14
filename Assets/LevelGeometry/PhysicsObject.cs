using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PhysicsObject : DamageableComponent
{
    public List<AudioClip> BreakSounds;
    public List<AudioClip> DamageSounds;
    private Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    protected override void OnDamage(DamageInfo damageInfo)
    {
        switch (damageInfo.DamageType)
        {
            case DamageType.DeathRay:
                rigidbody.AddForce(damageInfo.ImpactLocation.position * 2f, ForceMode.Impulse);
                break;
            case DamageType.Plunger:
                rigidbody.AddForce(damageInfo.ImpactLocation.position * 3f, ForceMode.Impulse);
                break;
            case DamageType.Bullet:
                rigidbody.AddForce(damageInfo.ImpactLocation.position * 0.25f, ForceMode.Impulse);
                break;
            case DamageType.EnergyDischarge:
                rigidbody.AddForce(damageInfo.ImpactLocation.position * 2f, ForceMode.Impulse);
                break;
        }
        GetComponent<AudioSource>().PlayOneShot(DamageSounds[Random.Range(0, DamageSounds.Count)]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 8f)
        {
            Debug.Log($"Strong collision on {gameObject.name}: {collision.impulse.magnitude}");
            var damage = new DamageInfo(collision.impulse.magnitude * 2, this.gameObject, DamageType.Collision, transform);
            Damage(damage);
        }
    }

    protected override IEnumerator DeleteThis(float time)
    {
        this.GetComponentInChildren<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    protected override void OnBreak(DamageInfo damageInfo)
    {
        GetComponent<AudioSource>().PlayOneShot(BreakSounds[Random.Range(0, BreakSounds.Count)]);
        //By default just make a sound on break, but types that inherit from this like crates might instead splinter, once I get that working
        StartCoroutine(DeleteThis(5f));
    }
}
