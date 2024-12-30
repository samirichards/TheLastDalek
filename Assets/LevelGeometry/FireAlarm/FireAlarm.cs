using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireAlarm : DamageableComponent
{
    public delegate void FireAlarmHitHandeller(FireAlarm FireAlarm);
    public static event FireAlarmHitHandeller OnFireAlarmHit;
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

    protected override void OnBreak(DamageInfo damageInfo)
    {
        OnFireAlarmHit.Invoke(this);
    }
}
