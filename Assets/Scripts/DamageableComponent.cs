using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class DamageableComponent : MonoBehaviour
{
    [SerializeField] protected float MaxHealth = 10;
    [SerializeField] protected float Health;

    void Awake()
    {
        Health = MaxHealth;
    }

    public void Damage(DamageInfo damageInfo)
    {
        Health -= damageInfo.DamageValue;
        OnDamage(damageInfo);
        if (Health <= 0)
        {
            OnDestroy(damageInfo);
        }
    }

    /// <summary>
    /// Called on each instance of receiving damage after health is deducted
    /// </summary>
    /// <param name="damageInfo">Damage source information needs to be set</param>
    protected virtual void OnDamage(DamageInfo damageInfo)
    {
        Debug.Log("Default damage behavior, Damage came from: " + damageInfo.DamageSource.name);
    }

    /// <summary>
    /// Called at the same time as OnDamage if the health is less than or equal to 0
    /// </summary>
    /// <param name="damageInfo">This will be provided through calling of the Damage Function in the base class</param>
    protected virtual void OnDestroy(DamageInfo damageInfo)
    {
        Debug.Log("Default Destroy behavior, " + gameObject.name + " Destroyed");
        Destroy(gameObject);
    }
}
