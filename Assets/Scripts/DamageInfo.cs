using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public float DamageValue;
    public GameObject DamageSource;
    public DamageType DamageType;

    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
    }
}

public enum DamageType
{
    DeathRay = 0,
    Plunger = 1,
    Bullet = 2,
    EnergyDischarge = 3,
    Fire = 4,
    Collision = 5,
    Point = 6
}
