using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tardis : DamageableComponent
{
    [SerializeField] private AudioClip[] DamageSounds;
    [SerializeField] private AudioClip DestroyedStinger;
    [SerializeField] private GameObject ExplosionPrefab;

    void Awake()
    {
        MaxHealth = 500;
    }

    protected override void OnDamage(DamageInfo damageInfo)
    {
        base.OnDamage(damageInfo);
        GetComponent<AudioSource>().PlayOneShot(DamageSounds[Random.Range(0, DamageSounds.Length)]);
    }

    protected override void OnBreak(DamageInfo damageInfo)
    {
        Instantiate(ExplosionPrefab, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(DestroyedStinger, transform.position);
        Destroy(gameObject);
    }
}
