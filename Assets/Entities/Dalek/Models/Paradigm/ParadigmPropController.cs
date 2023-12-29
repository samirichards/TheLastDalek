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
    [SerializeField] public ParadigmRank rank = ParadigmRank.Drone;
    [SerializeField] private MeshRenderer[] skinnedComponents;
    [SerializeField] private Material[] rankSkins;


    public enum ParadigmRank
    {
        Drone = 0,
        Scientist = 1,
        Strategist = 2,
        Eternal = 3,
        Supreme = 4
    }

    public override void StartBehavior()
    {
        SetSkin();
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        SetSkin();
    }

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

    public void SetSkin()
    {
        foreach (var mesh in skinnedComponents)
        {
            mesh.materials = new Material[] { rankSkins[(int)rank] };
        }
    }
}
