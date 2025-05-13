using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Header("Door Controls")]
    [SerializeField] private bool OpenDoorsOnRoomClear = false;
    [SerializeField] private List<DoorInteractionBehavior> ControlledDoors;
    [Header("Fire Alarm Stuff")]
    [SerializeField] public bool IsLevelWaterlogged = false;
    [SerializeField] public GameObject? Water;
    [SerializeField] public AudioSource? FireAlarmAudio;
    [SerializeField] public GameObject? Sprinklers;
    // Start is called before the first frame update
    void Start()
    {
        IsLevelWaterlogged = false;
        if (Water != null)
        {
            Water.SetActive(false);
        }
        if (Sprinklers != null)
        {
            Sprinklers.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSceneIsWaterlogged()
    {
        IsLevelWaterlogged = true;
        if (Water != null)
        {
            Water.SetActive(true);
        }
    }

    void Awake()
    {
        BaseAI.OnNPCDeath += NPCKilledInScene;
        FireAlarm.OnFireAlarmHit += FireAlarmHit;
    }

    private void OnDestroy()
    {
        FireAlarm.OnFireAlarmHit -= FireAlarmHit;
        BaseAI.OnNPCDeath -= NPCKilledInScene;
    }

    private void FireAlarmHit(FireAlarm fireAlarm)
    {
        SetSceneIsWaterlogged();
        //Start some fire alarms and also trigger the levels sprinklers
        if (Sprinklers != null)
        {
            Sprinklers.SetActive(true);
        }
        if (FireAlarmAudio != null)
        {
            FireAlarmAudio.Play();
        }
    }

    public void UnlockAllDoors()
    {
        Debug.Log("Unlocking all controlled doors in scene");
        foreach (DoorInteractionBehavior door in ControlledDoors)
        {
            door.SetIsOpen(true);
        }
    }

    private void NPCKilledInScene(BaseAI npc)
    {
        if (IsLevelWaterlogged)
        {
            //If the floor is wet, kill everything
            foreach (var item in FindObjectsOfType<BaseAI>().Where(a => a.IsAlive == true))
            {
                item.Damage(new DamageInfo(item.Health, gameObject, DamageType.Electric));
            }
        }
        if (!FindObjectsOfType<BaseAI>().Any(a => a.IsAlive == true) && OpenDoorsOnRoomClear)
        {
            UnlockAllDoors();
        }
    }
}
