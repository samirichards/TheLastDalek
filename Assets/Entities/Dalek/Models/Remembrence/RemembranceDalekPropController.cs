using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemembranceDalekPropController : PropController
{
    [Header("Remembrance Specific Properties")]
    [SerializeField] private Material inactiveEmittersMaterial;
    [SerializeField] private Material activeEmittersMaterial;
    [SerializeField] private MeshRenderer emitters;

    [Header("Death animation")]
    [SerializeField] private GameObject Gib_Eyestalk;
    [SerializeField] private GameObject Gib_Head;
    [SerializeField] private GameObject Gib_Neck;
    [SerializeField] private GameObject ExplosionPrefab;
    public override void SetEmittersActive(bool state)
    {
        if (state)
        {
            emitters.materials = emitters.materials.Where(a => a.name != inactiveEmittersMaterial.name).Append(activeEmittersMaterial).ToArray();
        }
        else
        {
            emitters.materials = emitters.materials.Where(a => a.name != activeEmittersMaterial.name).Append(inactiveEmittersMaterial).ToArray();
        }
    }

    public void ANIMATION_Death_CreateGibs()
    {
        var eyeStalkGib = Instantiate(Gib_Eyestalk, getEyeStalkObject.transform.position, getEyeStalkObject.transform.rotation);
        var headGib = Instantiate(Gib_Head, getHeadObject.transform.position, getHeadObject.transform.rotation);
        Transform neckTransform = getHeadObject.transform;
        var temp = neckTransform.position;
        temp.y = temp.y - 0.33f;
        neckTransform.position = temp;
        var neckGib = Instantiate(Gib_Neck, neckTransform.position, neckTransform.rotation);
        eyeStalkGib.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 1.33f);
        eyeStalkGib.GetComponent<Rigidbody>().AddRelativeForce(Random.onUnitSphere * 0.5f);
        headGib.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 1.25f);
        headGib.GetComponent<Rigidbody>().AddRelativeForce(Random.onUnitSphere * 0.66f);
        neckGib.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 1.1f);
        neckGib.GetComponent<Rigidbody>().AddRelativeForce(Random.onUnitSphere * 1f);
        Instantiate(ExplosionPrefab, getCenterObject.transform.position, getCenterObject.transform.rotation);
    }
}
