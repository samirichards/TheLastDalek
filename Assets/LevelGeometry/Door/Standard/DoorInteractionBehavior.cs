using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionBehavior : InteractionBehavior
{
    [SerializeField]public bool IsOpen = false;
    public override void Interact(GameObject interactingCharacter)
    {
        IsOpen = !IsOpen;
    }

    void FixedUpdate()
    {
        GetComponent<Animator>().SetBool("Door_IsOpen", IsOpen);
    }
}
