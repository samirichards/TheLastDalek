using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevateSceneChangeDoorInteractionBehavior : SceneChangeDoorInteractionBehavior
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && DoorEnabled)
        {
            if (Player._inventoryController.EquippedItems.Any(a=>a.ItemTitle == "Elevate"))
            {
                Player._movement.IsElevating = true;
                GameManager.GetLevelTransitionManager().ChangeScene(TargetSceneLoadLocation, TargetSceneName);
            }
        }
    }
}
