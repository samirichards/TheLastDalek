using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public abstract class DamageableComponent : MonoBehaviour
{
    [SerializeField] protected float MaxHealth = 10;
    [SerializeField] protected float Health;

    void Start()
    {
        Health = MaxHealth;
    }

    public void Damage(DamageInfo damageInfo)
    {
        Health -= damageInfo.DamageValue;
        OnDamage(damageInfo);
        if (Health <= 0)
        {
            OnBreak(damageInfo);
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
    protected virtual void OnBreak(DamageInfo damageInfo)
    {
        Debug.Log("Default Destroy behavior, " + gameObject.name + " Destroyed after 5 seconds");
        StartCoroutine(DeleteThis(5f));
    }

    protected virtual IEnumerator DeleteThis(float time)
    {
        this.GetComponentInChildren<MeshRenderer>().enabled = false;
        this.GetComponentInChildren<VisualEffect>().Stop();
        this.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
