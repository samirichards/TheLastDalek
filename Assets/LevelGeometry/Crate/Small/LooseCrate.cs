using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseCrate : PhysicsObject
{
    [SerializeField] private GameObject PackingPeanutsPrefab;
    protected override void OnBreak(DamageInfo damageInfo)
    {
        GetComponent<AudioSource>().PlayOneShot(BreakSounds[Random.Range(0, BreakSounds.Count)]);
        var collision = Instantiate(PackingPeanutsPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DeleteThis(5f));
    }
}
