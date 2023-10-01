using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExplosionManager
{
    [SerializeField] static public GameObject SmallExplosionPrefab;
    [SerializeField] static public GameObject LargeExplosionPrefab;



    public enum ExplosionSize
    {
        Small,
        Large
    }


    public static void CreateExplosion(Vector3 location, ExplosionSize size, bool damageSurroundings)
    {
        switch (size)
        {
            case ExplosionSize.Small:
                break;
            case ExplosionSize.Large:
                break;
            default:
                break;
        }
    }


}
