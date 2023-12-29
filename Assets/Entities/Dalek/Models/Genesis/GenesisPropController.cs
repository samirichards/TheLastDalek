using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenesisPropController : PropController
{
    [Header("Genesis Specific Properties")]
    [SerializeField] private Material inactiveEmittersMaterial;
    [SerializeField] private Material activeEmittersMaterial;
    [SerializeField] private MeshRenderer emitters;
    public override void SetEmittersActive(bool state)
    {
        if (state)
        {
            emitters.materials = new Material[] { activeEmittersMaterial };
        }
        else
        {
            emitters.materials = new Material[] { inactiveEmittersMaterial };
        }
    }
}
