using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireAlarm : DamageableComponent
{
    void Awake()
    {
        MaxHealth = 1;
        Health = MaxHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnDestroy(DamageInfo damageInfo)
    {
        Debug.Log("Fire alarm destroyed, let's get wet");
    }
}
