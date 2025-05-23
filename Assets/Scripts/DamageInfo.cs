using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public float DamageValue;
    public GameObject DamageSource;
    public DamageType DamageType;
    public bool DestroyTarget;
    public Color DissolveColour;
    public Transform ImpactLocation;

    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
        DestroyTarget = false;
    }

    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType, Transform _impactLocation)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
        DestroyTarget = false;
        ImpactLocation = _impactLocation;
    }
    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType, bool _destroyTarget)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
        DestroyTarget = _destroyTarget;
    }

    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType, bool _destroyTarget, Transform _impactLocation)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
        DestroyTarget = _destroyTarget;
        ImpactLocation = _impactLocation;
    }

    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType, bool _destroyTarget, Color _dissolveColour)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
        DestroyTarget = _destroyTarget;
        DissolveColour = _dissolveColour;
    }

    public DamageInfo(float damageValue, GameObject damageSource, DamageType damageType, bool _destroyTarget, Color _dissolveColour, Transform _impactLocation)
    {
        DamageValue = damageValue;
        DamageSource = damageSource;
        DamageType = damageType;
        DestroyTarget = _destroyTarget;
        DissolveColour = _dissolveColour;
        ImpactLocation = _impactLocation;
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
    Point = 6,
    Electric = 7,
    Explosion = 8
}
