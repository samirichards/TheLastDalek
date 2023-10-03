using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private bool OpenDoorsOnRoomClear = false;
    [SerializeField] private List<DoorInteractionBehavior> ControlledDoors;
    [SerializeField] public bool IsLevelWaterlogged = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSceneIsWaterlogged()
    {
        IsLevelWaterlogged = true;
    }

    void Awake()
    {
        BaseAI.OnNPCDeath += NPCKilledInScene;
    }

    public void UnlockAllDoors()
    {
        Debug.Log("Unlocking all controlled doors in scene");
        foreach (DoorInteractionBehavior door in ControlledDoors)
        {
            door.IsOpen = true;
        }
    }

    private void NPCKilledInScene(BaseAI npc)
    {
        if (!Object.FindObjectsOfType<BaseAI>().Any(a => a.IsAlive == true) && OpenDoorsOnRoomClear)
        {
            UnlockAllDoors();
        }
    }
}
