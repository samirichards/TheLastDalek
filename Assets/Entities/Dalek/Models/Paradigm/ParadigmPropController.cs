using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParadigmPropController : PropController
{
    [Header("Paradigm Specific Properties")]
    [SerializeField] private Material inactiveEmittersMaterial;
    [SerializeField] private Material activeEmittersMaterial;
    [SerializeField] private MeshRenderer emitters;
    public override void SetEmittersActive(bool state)
    {
        if (state)
        {
            emitters.materials = new Material[] { activeEmittersMaterial};
        }
        else
        {
            emitters.materials = new Material[] { inactiveEmittersMaterial };
        }
    }
}
