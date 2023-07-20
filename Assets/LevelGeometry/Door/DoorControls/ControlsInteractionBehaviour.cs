using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsInteractionBehaviour : InteractionBehavior
{
    public DoorInteractionBehavior ControlledDoor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Interact(GameObject interactingCharacter)
    {
        ControlledDoor.Interact(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
