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
}
