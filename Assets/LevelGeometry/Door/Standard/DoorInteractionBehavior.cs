using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionBehavior : InteractionBehavior
{
    [SerializeField] protected bool IsOpen = false;
    public virtual void SetIsOpen(bool _value)
    {
        IsOpen = _value;
    }
    public virtual bool GetIsOpen()
    {
        return IsOpen;
    }
    public override void Interact(GameObject interactingCharacter)
    {
        IsOpen = !IsOpen;
    }

    void FixedUpdate()
    {
        GetComponent<Animator>().SetBool("Door_IsOpen", IsOpen);
    }
}
