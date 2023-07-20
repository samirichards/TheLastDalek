using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionBehavior : MonoBehaviour
{
    public virtual void Interact(GameObject interactingCharacter)
    {
        Debug.Log("Default interaction behavior, Interacted with: " + interactingCharacter.name);
    }
}
