using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevateSceneChangeDoorInteractionBehavior : DoorInteractionBehavior
{
    [SerializeField] public BoxCollider SceneChangeTrigger;
    [SerializeField] public string TargetSceneName;
    [SerializeField] public Vector3 TargetSceneLoadLocation;
    public override void Interact(GameObject interactingCharacter)
    {

    }

    void FixedUpdate()
    {
        SceneChangeTrigger.enabled = IsOpen;
        GetComponent<Animator>().SetBool("Door_IsOpen", IsOpen);
        SceneChangeTrigger.enabled = IsOpen;
    }

    void Awake()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Player._inventoryController.EquippedItems.Any(a=>a.ItemTitle == "Elevate"))
            {
                Player._movement.IsElevating = true;
                GameManager.GetLevelTransitionManager().ChangeScene(TargetSceneLoadLocation, TargetSceneName);
            }
        }
    }
}
